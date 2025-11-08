# Pawn to Don - Project Contribution Documentation

This document outlines my core contributions to the "Pawn to Don" project, a Unity-based game leveraging a rich dialogue system, cinematic management, and diverse gameplay elements.

## Project Setup & Architecture

- **Initial Structure:** I was responsible for setting up the core project structure in Unity, preparing the folder architecture, main scenes, and asset pipelines for scalable development.
- **Integrations:** Ensured proper integration of major plugins and external codebases such as Pixel Crushers' Dialogue System.

## Dialogue System

- **Dialogue Tree Creation:** Utilized Pixel Crushers' Dialogue System to author and visually construct complex dialogue trees (see [DialogueEditorWindowDialogueTreeSection.cs](https://github.com/umangfuncell/pawn_to_don/blob/d30a172a7bda030c33ea88fa56ef6b0dcd2f5fb2/Assets/Plugins/Pixel%20Crushers/Dialogue%20System/Scripts/Editor/Dialogue%20Editor/DialogueEditorWindowDialogueTreeSection.cs)), both through manual editing and importing from tools like Articy and Celtx (see importer scripts).
- **Dialogue Integration:** Developed workflows to integrate authored dialogue trees into actual game logic, ensuring conversations flow between actors and react to game states via [Conversation.cs](https://github.com/umangfuncell/pawn_to_don/blob/d30a172a7bda030c33ea88fa56ef6b0dcd2f5fb2/Assets/Plugins/Pixel%20Crushers/Dialogue%20System/Scripts/MVC/Model/Data/Conversation.cs) and [DialogueEntry.cs](https://github.com/umangfuncell/pawn_to_don/blob/d30a172a7bda030c33ea88fa56ef6b0dcd2f5fb2/Assets/Plugins/Pixel%20Crushers/Dialogue%20System/Scripts/MVC/Model/Data/DialogueEntry.cs).
- **Editor Extension:** Ensured custom editor workflows for efficiently outlining, organizing, and testing dialogue trees ([DialogueEditorWindowDialogueTreeSection.cs], `BuildDialogueTree()`, and related node/link structures).

## Cutscene and Timeline Management

- **Cutscene Manager:** Developed or integrated a Cutscene Manager system to trigger and control Unity Timeline sequences, synchronizing animations, events, and camera transitions based on story progression.
- **Event Systems:** Linked dialogue and gameplay state changes to timeline triggers for seamless cinematic experiences.

## Mini Games and Level Management

- **Mini Games:** Created several mini-game modules, encapsulating their logic and UI into reusable Unity prefabs and scripts.
- **Level Managers:** Designed and implemented level management components to coordinate objectives, transitions, and save/load flow, interfacing these with the overarching game state and dialogue events.

## User Interface Implementation

- **UI Implementation:** Led UI prototyping and development for dialogues, menus, cutscene overlays, and mini-game interfaces to provide cohesive player feedback.
- **Integration:** Built bridges between UI elements and game logic, enabling real-time updates, event-driven changes, and dialogue interaction.

## Technologies & Languages

- **Primary Languages:** C# (Unity scripts/game logic/UI), with ancillary use of C (for plugin integrations), HTML/JavaScript (UI and web-based interactions), C++/Objective-C (platform extensions).
- **Plugins Used:** Pixel Crushers Dialogue System, Articy/Celtx importers for narrative assets, Unity Timeline.

## Summary

Through architectural setup, narrative system integration, cutscene and timeline management, mini-game development, level managers, and UI implementation, I played a central role in delivering a flexible, interactive, and narratively rich gameplay experience.

---

*For further details or code review, see referenced files and dialogue system editor scripts within the repository.*

PLAY GAME:  https://play.google.com/store/apps/details?id=com.funcell.pawndon&hl=en
