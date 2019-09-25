Invoke-WebRequest https://raw.githubusercontent.com/Microsoft/PowerBI-CSharp/master/sdk/swaggers/swaggerV2.json -o "powerbi.json"
autorest "--input-file=powerbi.json" --csharp --namespace=SceneSkope.PowerBI --output-folder=SceneSkope.PowerBI --add-credentials
