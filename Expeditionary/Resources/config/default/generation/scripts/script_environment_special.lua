luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

MapEnvironmentModifier=luanet.import_type('Expeditionary.Model.Mapping.MapEnvironmentModifier')

Volcanic = MapEnvironmentModifier()
Volcanic.Key = "environment-modifier-special-volcanic"
Volcanic.Name = "Volcanic"
function Volcanic:Apply(environment)
	local appearance = environment.Appearance;
	local parameters = environment.Parameters
	local terrain = parameters.Terrain

	terrain.Stone.C = 1
	appearance.Soil.C = StaticColoring(Color4(0.23, 0.23, 0.23, 1))
	appearance.GroundCover = StaticColoring(Color4(0.66, 0.66, 0.66, 1))
end

function Load()
	return { Volcanic }
end