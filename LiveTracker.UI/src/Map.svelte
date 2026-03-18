<script>
  import { onMount } from "svelte";
  import * as signalR from "@microsoft/signalr";
  import L from "leaflet";
  import "leaflet/dist/leaflet.css";

  let map;
  let marker;
  let isInitialized = false;
  let lastUpdate = Date.now();
  let connectionStatus = "Connecting...";

  // Config from .env
  const signalRHubUrl = import.meta.env.VITE_SIGNALR_HUB_URL;
  const defaultZoom = parseInt(import.meta.env.VITE_MAP_DEFAULT_ZOOM);
  const tileUrl = import.meta.env.VITE_MAP_TILE_URL;
  const attribution = import.meta.env.VITE_MAP_ATTRIBUTION;

  // Function to smoothly animate marker movement
  let currentPos = null;
  function animateMarker(newLat, newLng, duration = 1000) {
    if (!marker) return;

    const frames = 60; // number of animation frames
    const startLat = currentPos ? currentPos[0] : newLat;
    const startLng = currentPos ? currentPos[1] : newLng;
    let i = 0;

    function step() {
      i++;
      const t = i / frames;
      const lat = startLat + (newLat - startLat) * t;
      const lng = startLng + (newLng - startLng) * t;
      marker.setLatLng([lat, lng]);
      map.panTo([lat, lng], { animate: true });
      if (i < frames) requestAnimationFrame(step);
      else currentPos = [newLat, newLng];
    }

    step();
  }

  onMount(async () => {
    // Create map without initial coordinates
    map = L.map("map");
    L.tileLayer(tileUrl, { attribution }).addTo(map);

    // Set up SignalR connection with automatic reconnect
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(signalRHubUrl)
      .withAutomaticReconnect([0, 2000, 5000, 10000])
      .build();

    // Handle reconnecting event
    connection.onreconnecting((err) => {
      connectionStatus = "Reconnecting...";
      console.warn("SignalR reconnecting", err);
    });

    // Handle reconnected event
    connection.onreconnected((id) => {
      connectionStatus = "Connected";
      console.log("SignalR reconnected:", id);
    });

    // Handle connection closed event
    connection.onclose((err) => {
      connectionStatus = "Disconnected";
      console.error("SignalR connection closed", err);
    });

    // Listen for location updates from server
    connection.on("ReceiveLocation", (lat, lng) => {
      lastUpdate = Date.now();

      if (!isInitialized) {
        // First location → initialize map and marker
        map.setView([lat, lng], defaultZoom);
        marker = L.marker([lat, lng]).addTo(map);
        currentPos = [lat, lng];
        isInitialized = true;
        connectionStatus = "Connected";
        return;
      }

      // Animate marker to new coordinates
      animateMarker(lat, lng);
    });

    // Start SignalR connection
    await connection.start();

    // Timer to detect if no data is received for a while
    setInterval(() => {
      if (Date.now() - lastUpdate > 5000) {
        connectionStatus = "No data from server";
      }
    }, 1000);
  });
</script>

<div>
  <div id="map"></div>
  <p>Status: {connectionStatus}</p>
</div>

<style>
  #map {
    height: 500px;
    width: 100%;
  }
  p {
    margin-top: 0.5rem;
    font-weight: bold;
  }
</style>