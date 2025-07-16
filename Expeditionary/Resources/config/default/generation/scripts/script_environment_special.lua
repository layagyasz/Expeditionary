luanet.load_assembly('Expeditionary', 'Expeditionary.Model')
luanet.load_assembly('OpenTK.Mathematics', 'OpenTK.Mathematics')

Color4=luanet.import_type('OpenTK.Mathematics.Color4')
MapEnvironmentModifier=luanet.import_type('Expeditionary.Model.Mapping.MapEnvironmentModifier')
StaticColoring=luanet.import_type('Expeditionary.Model.Mapping.Appearance.IColoring+StaticColoring')

Volcanic = MapEnvironmentModifier()
Volcanic.Key = "environment-modifier-special-volcanic"
Volcanic.Name = "Volcanic"
function Volcanic:Apply(environment)
	local appearance = environment.Appearance
	local parameters = environment.Parameters
	local terrain = parameters.Terrain

	terrain.Stone.C = 1
	appearance.Soil.C = StaticColoring(Color4(0.23, 0.23, 0.23, 1))
	appearance.GroundCover = StaticColoring(Color4(0.66, 0.66, 0.66, 1))
end

function Load()
	return { Volcanic }
end