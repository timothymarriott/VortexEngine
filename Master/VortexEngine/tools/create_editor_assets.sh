

VortexEngineCLI=../VortexEngineCLI/bin/publish/osx-arm64/VortexEngineCLI

$VortexEngineCLI pack -i ../EditorData -o editor_data.pkk
$VortexEngineCLI pack -i ../VortexEnginePlayer/bin/publish/osx-arm64/ -o editor_data.pkk -r ../VortexEnginePlayer/bin