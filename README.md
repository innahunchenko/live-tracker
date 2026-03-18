# live-tracker

LiveTracker is a real-time tracking application. 
The frontend is built with **Svelte** and displays a map with a moving marker. 
The backend is an **ASP.NET Core SignalR** service sending coordinates, running in a Docker container.

## Features

- Real-time map updates with smooth marker animation
- Marker rotates based on route direction
- SignalR connection with automatic reconnect

## Getting Started

### Backend

1. Build and run backend in Docker:

```bash
docker build -t livetracker-backend ./backend
docker run -p 32771:80 livetracker-backend

### Frontend

## Install dependencies

```bash
npm install

Then open your browser at:
http://localhost:5173
