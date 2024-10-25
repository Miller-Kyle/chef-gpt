$markdownContent = Get-Content -Path "gpt-instructions-safety.md" -Raw

# Replace Markdown headers with formatted text
$escapedContent = $markdownContent -replace "`r?`n", "\n" `
                                    -replace "'", "\'" `
                                    -replace "#+\s*", "" # Remove Markdown headers

$escapedContent