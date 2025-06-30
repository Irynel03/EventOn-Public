# EventOn Mobile App

**EventOn** is a complete mobile solution for event access and engagement. Developed for both Android, IOS and Desktop, this application provides users with an intuitive platform to discover, attend, and interact with events.

## Overview

The EventOn app aims to deliver a comprehensive digital experience, moving beyond simple ticket purchasing. It incorporates personalized recommendations, social features, and offline capabilities to create a connected and user-friendly environment for event-goers.

### Key Features

* **Event Feed**: A dynamic and personalized feed displaying current and recommended events.
* **Advanced Search & Filtering**: Easily find events by name, date, location, or price.
* **Personalized Recommendations**: An AI-powered system suggests events tailored to the user's interests and past activities.
* **Integrated Chatbot**: A conversational chatbot (powered by Google Gemini) to help users discover events.
* **Secure Ticketing**: In-app ticket purchasing powered by **Stripe**.
* **QR Code Access**: Digital tickets with unique QR codes for easy and secure event entry.
* **Offline Access**: Purchased tickets and QR codes are stored locally for access without an internet connection.
* **Social Interaction**: Like and comment on events, follow favorite artists and organizers.
* **Push Notifications**: Receive real-time alerts for new events from followed artists, comments, and more.
* **Google Maps Integration**: View event locations and get directions directly within the app.
* **User-Friendly Interface**: A clean, responsive design with both light and dark modes.

## Tech Stack

* **Framework**: .NET MAUI 9 Hybrid Blazor (Multi-platform App UI)
* **Language**: C#
* **Architecture**: MVVM (Model-View-ViewModel)
* **Local Storage**: Secure Storage for sensitive data like authentication tokens and Preferences for offline ticket data.

## How It Works

The mobile application communicates with the **EventOn.API** for all its data needs. All business logic, from user authentication to payment processing, is handled by the backend, ensuring the mobile client remains lightweight and focused on the user experience.
