@echo off
C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /target:winexe /out:PongGame.exe Program.cs PongGame.cs SoundGenerator.cs HighscoreManager.cs
if %errorlevel% neq 0 (
    echo Compilation Failed!
    pause
    exit /b %errorlevel%

:: Commit step 91 of 150
