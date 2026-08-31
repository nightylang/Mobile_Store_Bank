# CONFIGURATION - CHANGE THESE TO YOUR GITHUB DETAILS
$GitHubUser = "YOUR_GITHUB_USERNAME"
$RepoName = "Mobile_Store_Bank"

Write-Host "📝 Creating .gitignore configuration layer..." -ForegroundColor Yellow
@'
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
'@ | Out-File -FilePath .gitignore -Encoding utf8

Write-Host "🚀 Initializing Git repository mapping..." -ForegroundColor Yellow
git init

Write-Host "📦 Staging system infrastructure files..." -ForegroundColor Yellow
git add .

Write-Host "💾 Committing source framework baseline..." -ForegroundColor Yellow
git commit -m "feat: core .NET 10.0 HTTP architecture with Glassmorphism + SaaS layout"

Write-Host "🌿 Setting tracking branch designation to main..." -ForegroundColor Yellow
git branch -M main

Write-Host "🔗 Binding upstream remote GitHub address..." -ForegroundColor Yellow
git remote add origin "https://github.com"

Write-Host "📤 Executing upstream code transmission push..." -ForegroundColor Yellow
git push -u origin main

Write-Host "✅ Code infrastructure successfully dispatched to GitHub!" -ForegroundColor Green
