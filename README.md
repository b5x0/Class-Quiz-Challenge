# ClassQuiz Challenge

## Project Overview

This repository contains a completed technical challenge for a Game Developer position. The project is a "match the picture to the name" quiz game built entirely in **Unity** with **C#**. It was developed to showcase core game development skills, including UI implementation, game state management, and adherence to specific design requirements.

---

## Gameplay & Features

The game consists of 10 rounds where the player must correctly match three pictures of animals to their corresponding names by drawing lines between them.

 
*(**Note:** I've used a placeholder image here. You should take a screenshot of YOUR game and replace this URL to make it look even better! You can upload an image directly to a GitHub issue or use a site like Imgur.)*

### Core Features:
*   **Drag & Drop Line Drawing:** Intuitive line-drawing mechanic to connect images and names.
*   **Dynamic Round Generation:** Each round randomly selects 3 unique items from a pool of 30, ensuring no repeats within a game session.
*   **Timer & Attempts System:** A 1-minute countdown timer per round adds a sense of urgency. The final score is based on the number of attempts.
*   **Scoring & Progression:**
    *   Players earn stars (0-3) based on the number of attempts.
    *   Coins are awarded for each completed round, with bonuses for higher star counts.
    *   A progress bar tracks the player's progression through the 10-round challenge.
*   **Responsive UI:** The UI is built to scale correctly across different screen resolutions and aspect ratios using Unity's Canvas Scaler.
*   **Complete Game Loop:** Features a start, a win/lose condition per round, a "Game Over" screen, and a "Play Again" option.

---

## Technical Implementation

*   **Engine:** Unity 2022.3.x LTS (or your specific version)
*   **Language:** C#
*   **Key Unity Systems Used:**
    *   **UI Toolkit / uGUI:** For all visual elements, including popups, buttons, and sliders.
    *   **ScriptableObjects:** To create a scalable and designer-friendly system for quiz item data (names and sprites).
    *   **EventSystem (IPointer Handlers):** Managed all drag-and-drop user input for a robust and decoupled interaction system.
    *   **Unity UI Extensions:** Utilized for the `UILineRenderer` component to draw lines on the canvas.

## How to Run the Project

1.  Clone the repository: `git clone https://github.com/b5x0/Class-Quiz-Challenge.git`
2.  Open the project folder in Unity Hub.
3.  Launch the project in the Unity Editor.
4.  The main scene is located in `Assets/Scenes/`.

Alternatively, an **APK** for direct installation on Android devices has been provided separately.

---
