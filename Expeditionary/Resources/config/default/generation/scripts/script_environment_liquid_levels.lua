luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

MapEnvironmentTrait=luanet.import_type('Expeditionary.Model.Mapping.Environments.MapEnvironmentTrait')

Islands = MapEnvironmentTrait()
Islands.Key = "environment-trait-liquid-level-islands"
function Islands:Apply(environment)
	 environment.Parameters.Terrain.LiquidLevel = 0.5
end

Coast = MapEnvironmentTrait()
Coast.Key = "environment-trait-liquid-level-coast"
function Coast:Apply(environment)
	 environment.Parameters.Terrain.LiquidLevel = 0.25
end

Lakes = MapEnvironmentTrait()
Lakes.Key = "environment-trait-liquid-level-lakes"
function Lakes:Apply(environment)
	 environment.Parameters.Terrain.LiquidLevel = 0.1
end

Inland = MapEnvironmentTrait()
Inland.Key = "environment-trait-liquid-level-inland"
function Inland:Apply(environment)
	 environment.Parameters.Terrain.LiquidLevel = 0
end

function Load()
	return { Islands, Coast, Lakes, Inland }
end