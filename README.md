## Cloning This Project

This Unity project uses Git LFS (Large File Storage) to handle large binary assets like textures, models, and audio files. Follow these steps to properly clone the repository:

### Prerequisites

Make sure you have Git LFS installed on your system:
```bash
git lfs install
```

### Cloning Instructions

**Method 1: Standard Clone (Recommended)**
```bash
git clone https://github.com/petaaar88/Stalker-Behaviour-UI.git
cd your-project-name
```

**Method 2: If LFS files don't download automatically**
```bash
git clone https://github.com/petaaar88/Stalker-Behaviour-UI.git
cd your-project-name
git lfs pull
```

**Method 3: Fast clone (skip LFS initially)**
```bash
GIT_LFS_SKIP_SMUDGE=1 git clone https://github.com/petaaar88/Stalker-Behaviour-UI.git
cd your-project-name
git lfs pull
```

### After Cloning

1. Open Unity Hub and add the cloned project folder
2. Open the project with Unity version `X.X.X` (check `ProjectSettings/ProjectVersion.txt` for the exact version)
3. Wait for Unity to import all assets

### Verification

To verify that LFS files were downloaded correctly:
```bash
git lfs ls-files
```

All large binary files should show their actual file sizes, not just a few bytes (which would indicate pointer files).
