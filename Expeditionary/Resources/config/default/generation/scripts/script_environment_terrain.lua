luanet.load_assembly('Expeditionary', 'Expeditionary.Model')
luanet.load_assembly('OpenTK.Mathematics', 'OpenTK.Mathematics')

MapEnvironmentTrait=luanet.import_type('Expeditionary.Model.Mapping.Environments.MapEnvironmentTrait')
Vector3=luanet.import_type('OpenTK.Mathematics.Vector3')

VeryRough = MapEnvironmentTrait()
VeryRough.Key = "environment-trait-terrain-very-rough"
function VeryRough:Apply(environment)
	environment.Parameters.Terrain.ElevationLayer.Noise.Frequency = Vector3(0.02, 0.02, 0.02)
end

Rough = MapEnvironmentTrait()
Rough.Key = "environment-trait-terrain-rough"
function Rough:Apply(environment)
	environment.Parameters.Terrain.ElevationLayer.Noise.Frequency = Vector3(0.015, 0.015, 0.015)
end

Rugged = MapEnvironmentTrait()
Rugged.Key = "environment-trait-terrain-rugged"
function Rugged:Apply(environment)
	environment.Parameters.Terrain.ElevationLayer.Noise.Frequency = Vector3(0.01, 0.01, 0.01)
end

Flat = MapEnvironmentTrait()
Flat.Key = "environment-trait-terrain-flat"
function Flat:Apply(environment)
	environment.Parameters.Terrain.ElevationLayer.Noise.Frequency = Vector3(0.0075, 0.0075, 0.0075)
end

VeryFlat = MapEnvironmentTrait()
VeryFlat.Key = "environment-trait-terrain-very-flat"
function VeryFlat:Apply(environment)
	environment.Parameters.Terrain.ElevationLayer.Noise.Frequency = Vector3(0.005, 0.005, 0.005)
end

SteepPlateau = MapEnvironmentTrait()
SteepPlateau.Key = "environment-trait-terrain-steep-plateau"
function SteepPlateau:Apply(environment)
	environment.Parameters.Terrain.ElevationLayer.StandardDeviation = 
		0.25 * environment.Parameters.Terrain.ElevationLayer.StandardDeviation
end

Plateau = MapEnvironmentTrait()
Plateau.Key = "environment-trait-terrain-plateau"
function Plateau:Apply(environment)
	environment.Parameters.Terrain.ElevationLayer.StandardDeviation = 
		0.5 * environment.Parameters.Terrain.ElevationLayer.StandardDeviation
end

Rolling = MapEnvironmentTrait()
Rolling.Key = "environment-trait-terrain-rolling"
function Rolling:Apply(environment)
end

Smooth = MapEnvironmentTrait()
Smooth.Key = "environment-trait-terrain-smooth"
function Smooth:Apply(environment)
	environment.Parameters.Terrain.ElevationLayer.StandardDeviation = 
		2 * environment.Parameters.Terrain.ElevationLayer.StandardDeviation
end

VerySmooth = MapEnvironmentTrait()
VerySmooth.Key = "environment-trait-terrain-very-smooth"
function VerySmooth:Apply(environment)
	environment.Parameters.Terrain.ElevationLayer.StandardDeviation = 
		4 * environment.Parameters.Terrain.ElevationLayer.StandardDeviation
end


function Load()
	return { VeryRough, Rough, Rugged, Flat, VeryFlat, SteepPlateau, Plateau, Rolling, Smooth, VerySmooth }
end