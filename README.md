# SettlementApi

## Overview

InfoTrack provides a settlement service where a property purchaser's conveyancer meets with representatives of the mortgage provider and the vendor's conveyancer at an agreed time. Due to fixed capacity, InfoTrack can only handle a limited number of simultaneous settlements. This API allows users to book settlement times and ensures the bookings comply with business rules and constraints.

## API Specification

### Endpoint

- **POST /api/bookings**

### Request Format

The request should be in JSON format:

```json
{
  "bookingTime": "09:30",
  "name": "John Smith"
}


### Usage
### Example Request

To make a booking, send a POST request to the API endpoint with the required data:

```bash
curl -X POST http://localhost:44352/api/bookings \
-H "Content-Type: application/json" \
-d '{"bookingTime": "10:30", "name": "Cristiano Ronaldo"}'

### Example Response

```json
{
    "bookingId": "a1222222-89av-fgfg-1234-56789abcdef0"
}

### Notes

- Bookings are stored in-memory, which means they will be lost if the application is restarted.
- The API adheres to the fixed capacity and business hours constraints as specified.

### Swagger

This app uses swagger so if we run the app using IIS Express then it automatically navigates to /swagger/index.html (or the full URL https://localhost:44352/swagger/index.html)

