luanet.load_assembly('Expeditionary', 'Expeditionary.Model')
luanet.load_assembly('OpenTK.Mathematics', 'OpenTK.Mathematics')

Color4=luanet.import_type('OpenTK.Mathematics.Color4')
MapEnvironmentTrait=luanet.import_type('Expeditionary.Model.Mapping.Environments.MapEnvironmentTrait')
StaticColoring=luanet.import_type('Expeditionary.Model.Mapping.Appearance.IColoring+StaticColoring')

NoOp = MapEnvironmentTrait()
NoOp.Key = "environment-trait-special-noop"
NoOp.Name = "NA"
function NoOp:Apply(environment)
end

Volcanic = MapEnvironmentTrait()
Volcanic.Key = "environment-trait-special-volcanic"
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
	return { NoOp, Volcanic }
end