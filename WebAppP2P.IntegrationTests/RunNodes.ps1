param(
	[switch]$CleanBuild,
	[switch]$OnlyBuild,
	[int[]]$Ports = @(5000,5001,5002)
)

foreach ($Port in $Ports) {
	$Path = "..\\WebAppP2P\bin\Release\Instance_" + $Port

	if($PSBoundParameters.ContainsKey('CleanBuild')){
		Write-Host "Clean build is performed at $($Path)"
		if(Test-Path $Path){
			Remove-Item -Path ( $Path + "\*" ) -Recurse
		}
		else{
			New-Item -ItemType "directory" -Path $Path
		}	
		Copy-Item ..\\WebAppP2P\bin\Release\PublishOutput\* -Destination $Path -Recurse
	}
	elseif(-Not (Test-Path $Path)){
		Write-Host "Build is performed at $($Path)"
		New-Item -ItemType "directory" -Path $Path
		Copy-Item ..\\WebAppP2P\bin\Release\PublishOutput\* -Destination $Path -Recurse
	}
	$nodeOptions = Get-Content ($Path + "\nodeOptions.json") -raw | ConvertFrom-Json
	$nodeOptions.self = "http://localhost:" + $Port + "/api/"
	$nodeOptions | ConvertTo-Json  | set-content ($Path + "\nodeOptions.json")
	if(-Not $PSBoundParameters.ContainsKey('OnlyBuild')){
		Write-Host "Running node $($Path) on port $($Port)"
		Start-Process powershell.exe -ArgumentList ("-command cd $($Path); dotnet .\WebAppP2P.dll port=$($Port)")
	}
}

