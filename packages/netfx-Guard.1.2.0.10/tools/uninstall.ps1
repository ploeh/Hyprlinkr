param($installPath, $toolsPath, $package, $project)

	gci $toolsPath\*.snippet | %{ remove-item (([System.Environment]::ExpandEnvironmentVariables("%VisualStudioDir%\Code Snippets\Visual C#\My Code Snippets\")) + $_.Name) -ErrorAction SilentlyContinue }