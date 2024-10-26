luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

MapEnvironmentModifier=luanet.import_type('Expeditionary.Model.Mapping.MapEnvironmentModifier')

Islands = MapEnvironmentModifier()
Islands.Key = "environment-modifier-liquid-level-islands"
Islands.Name = "Islands"
function Islands:Apply(environment)
	 environment.Parameters.Terrain.LiquidLevel = 0.5
end

Coast = MapEnvironmentModifier()
Coast.Key = "environment-modifier-liquid-level-coast"
Coast.Name = "Coast"
function Coast:Apply(environment)
	 environment.Parameters.Terrain.LiquidLevel = 0.25
end

Lakes = MapEnvironmentModifier()
Lakes.Key = "environment-modifier-liquid-level-lakes"
Lakes.Name = "Lakes"
function Lakes:Apply(environment)
	 environment.Parameters.Terrain.LiquidLevel = 0.1
end

Inland = MapEnvironmentModifier()
Inland.Key = "environment-modifier-liquid-level-inland"
Inland.Name = "Inland"
function Inland:Apply(environment)
	 environment.Parameters.Terrain.LiquidLevel = 0
end

function Load()
	return { Islands, Coast, Lakes, Inland }
end