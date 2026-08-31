#!/usr/bin/env bash
set -e

# CONFIGURATION - CHANGE THESE TO YOUR GITHUB DETAILS
GITHUB_USER="YOUR_GITHUB_USERNAME"
REPO_NAME="MobileStoreBank"

echo "📝 Creating .gitignore configuration layer..."
cat << 'EOF' > .gitignore
[Dd]ebug/
[Rr]elease/
bin/
obj/
*.db
*.db-journal
*.db-shm
*.db-wal
.vs/
.idea/
*.user
*.suo
.DS_Store
Thumbs.db
EOF

echo "🚀 Initializing Git repository mapping..."
git init

echo "📦 Staging system infrastructure files..."
git add .

echo "💾 Committing source framework baseline..."
git commit -m "feat: core .NET 10.0 HTTP architecture with Glassmorphism + SaaS layout"

echo "🌿 Setting tracking branch designation to main..."
git branch -M main

echo "🔗 Binding upstream remote GitHub address..."
# If using HTTPS:
git remote add origin "https://github.com{GITHUB_USER}/${REPO_NAME}.git"
# If you prefer SSH, uncomment the line below and comment the line above:
# git remote add origin "git@github.com:${GITHUB_USER}/${REPO_NAME}.git"

echo "📤 Executing upstream code transmission push..."
git push -u origin main

echo "✅ Code infrastructure successfully dispatched to GitHub!"
