# Team Setup Guide for Unity Project

To ensure everyone can work on the project without issues, please follow these steps:

## 1. Install Git LFS
- Download and install Git LFS from https://git-lfs.github.com/
- After installation, run this command in your terminal (only once per machine):
  
  git lfs install

## 2. Clone the Repository
- If you haven't cloned the repo yet, use:
  
  git clone <repo-url>

- If you already have the repo, just pull the latest changes:
  
  git pull

## 3. Open the Project in Unity
- Use the same Unity version as the rest of the team (check with the project lead or README).
- Open the project folder in Unity Hub or directly in the Unity Editor.

## 4. Additional Notes
- If you see files with the extension `.meta`, do not delete them—they are required by Unity.
- Do not commit files in Library/, Temp/, or other ignored folders.
- If you add new large assets (textures, models, audio, etc.), make sure they are tracked by Git LFS (this is automatic for common types).

## Troubleshooting
- If you see files with text like `version https://git-lfs.github.com/spec/v1`, it means Git LFS is not set up. Install Git LFS and run `git lfs pull`.
- For any issues, ask in the team chat or check the README for updates.

---

Happy coding!