
$env:AZURE_BASE_URL="https://openai-common-global.openai.azure.com/openai"
$env:AZURE_OPENAI_BASE_URL="https://openai-common-global.openai.azure.com/openai"


codex --provider azure --model gpt-4o-mini -a auto-edit --project-doc chinese-chess.feature -w "D:\Project\AIxBDD\AI-100x-SE-Join-Quest\ChineseChess"