$markdownContent = Get-Content -Path "gpt-instructions-safety.md" -Raw

# Replace Markdown headers with formatted text
$escapedContent = $markdownContent -replace "`r?`n", "\n" `
                                    -replace '"', '\"' `
                                    -replace "#+\s*", "" # Remove Markdown headers

$jsonString = '{ "text": "' + $escapedContent + '" }'
$jsonString

# Write the output to system-context.json
Set-Content -Path "system-context.json" -Value $jsonString