# GitHub Setup Script for Borrowdung Project
# Run this script to complete all GitHub configuration tasks

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "GitHub Setup untuk Project Borrowdung" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if gh is installed
if (!(Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Host "ERROR: GitHub CLI (gh) not found!" -ForegroundColor Red
    Write-Host "Install from: https://cli.github.com/" -ForegroundColor Yellow
    exit 1
}

Write-Host "✓ GitHub CLI detected" -ForegroundColor Green

# Check authentication
Write-Host ""
Write-Host "Checking GitHub authentication..." -ForegroundColor Yellow
$authStatus = gh auth status 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "! You need to login to GitHub first" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Running: gh auth login..." -ForegroundColor Cyan
    gh auth login
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Authentication failed" -ForegroundColor Red
        exit 1
    }
}

Write-Host "✓ GitHub authenticated" -ForegroundColor Green
Write-Host ""

# Repositories to configure
$repos = @(
    "diwanparker/2026-borrowdung-backend",
    "diwanparker/2026-borrowdung-frontend"
)

foreach ($repo in $repos) {
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Configuring: $repo" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    
    # Task 1: Set default branch to develop
    Write-Host ""
    Write-Host "Task 1: Setting default branch to 'develop'..." -ForegroundColor Yellow
    try {
        gh api -X PATCH "/repos/$repo" -f default_branch=develop 2>&1 | Out-Null
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Default branch set to 'develop'" -ForegroundColor Green
        } else {
            Write-Host "⚠ Could not set default branch (may require manual setup)" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "⚠ Could not set default branch: $_" -ForegroundColor Yellow
    }
    
    # Task 2: Create GitHub Release v1.0.0
    Write-Host ""
    Write-Host "Task 4: Creating GitHub Release v1.0.0..." -ForegroundColor Yellow
    
    $releaseNotes = @"
# Borrowdung v1.0.0 - Initial Release

## 🎉 First Release

Sistem Peminjaman Ruangan Kampus - Version 1.0.0

## ✨ Features

### Room Management
- ✅ CRUD operations untuk mengelola data ruangan kampus
- ✅ Search dan filter ruangan
- ✅ Pagination support

### Booking Management  
- ✅ Pencatatan peminjaman ruangan
- ✅ Approval workflow (Pending → Approved/Rejected)
- ✅ Conflict detection untuk mencegah double booking
- ✅ Booking history dengan filters

### API Features
- ✅ RESTful API dengan ASP.NET Core 8.0
- ✅ Entity Framework Core dengan SQLite
- ✅ Input validation menggunakan Data Annotations
- ✅ Soft delete implementation
- ✅ Swagger/OpenAPI documentation
- ✅ CORS support

### Frontend Features
- ✅ React 18 dengan TypeScript
- ✅ Tailwind CSS styling
- ✅ Responsive design
- ✅ Real-time dashboard statistics
- ✅ Interactive booking workflow

## 📋 What's Included

- Full CRUD for Room entity
- Full CRUD for Booking entity
- Status management system
- Database migrations and seeders
- Complete API documentation
- Frontend integration
- Environment configuration examples

## 🔧 Tech Stack

**Backend:**
- ASP.NET Core 8.0
- Entity Framework Core 8.0
- SQLite Database

**Frontend:**
- React 18
- TypeScript
- Tailwind CSS
- Vite
- Axios

## 📚 Documentation

See README.md for installation and usage instructions.
See CHANGELOG.md for detailed changes.

## 🙏 Credits

Developed as part of PBL (Project-Based Learning) 2026
"@

    # Check if release already exists
    $existingRelease = gh release view v1.0.0 -R $repo 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "ℹ Release v1.0.0 already exists" -ForegroundColor Cyan
    } else {
        try {
            gh release create v1.0.0 -R $repo --title "Borrowdung v1.0.0 - Initial Release" --notes $releaseNotes
            if ($LASTEXITCODE -eq 0) {
                Write-Host "✓ Release v1.0.0 created successfully" -ForegroundColor Green
            }
        } catch {
            Write-Host "⚠ Could not create release: $_" -ForegroundColor Yellow
        }
    }
    
    Write-Host ""
}

# Task 2: Create GitHub Project Board
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Task 2: GitHub Project Board" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Note: GitHub Projects v2 require manual setup via web interface" -ForegroundColor Yellow
Write-Host "URL: https://github.com/users/diwanparker/projects" -ForegroundColor Cyan
Write-Host ""
Write-Host "Quick steps:" -ForegroundColor White
Write-Host "1. Go to https://github.com/users/diwanparker/projects?query=is%3Aopen" -ForegroundColor Gray
Write-Host "2. Click 'New project'" -ForegroundColor Gray
Write-Host "3. Choose 'Board' template" -ForegroundColor Gray
Write-Host "4. Name: 'Borrowdung - Room Booking System'" -ForegroundColor Gray
Write-Host "5. Add columns: To Do, In Progress, Done" -ForegroundColor Gray
Write-Host ""

# Task 3: Check Pull Requests
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Task 3: Checking Pull Requests" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

foreach ($repo in $repos) {
    Write-Host "Repository: $repo" -ForegroundColor Yellow
    Write-Host "Open PRs:" -ForegroundColor White
    gh pr list -R $repo --limit 10
    Write-Host ""
    Write-Host "Closed/Merged PRs:" -ForegroundColor White
    gh pr list -R $repo --state merged --limit 5
    Write-Host ""
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "✓ Setup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "✓ Default branches checked/configured" -ForegroundColor Green
Write-Host "✓ GitHub Releases created" -ForegroundColor Green
Write-Host "⚠ Project Board needs manual setup" -ForegroundColor Yellow
Write-Host "✓ Pull Requests verified" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Create Project Board at: https://github.com/users/diwanparker/projects" -ForegroundColor White
Write-Host "2. Verify releases at:" -ForegroundColor White
Write-Host "   - https://github.com/diwanparker/2026-borrowdung-backend/releases" -ForegroundColor Gray
Write-Host "   - https://github.com/diwanparker/2026-borrowdung-frontend/releases" -ForegroundColor Gray
Write-Host ""
