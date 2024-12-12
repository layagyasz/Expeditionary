luanet.load_assembly('Expeditionary', 'Expeditionary.Model')
luanet.load_assembly('OpenTK.Mathematics', 'OpenTK.Mathematics')

MapEnvironmentModifier=luanet.import_type('Expeditionary.Model.Mapping.MapEnvironmentModifier')
Vector3=luanet.import_type('OpenTK.Mathematics.Vector3')

VeryRough = MapEnvironmentModifier()
VeryRough.Key = "environment-modifier-terrain-very-rough"
VeryRough.Name = "Very Rough"
function VeryRough:Apply(environment)
	environment.Parameters.Terrain.ElevationLayer.Noise.Frequency = Vector3(0.02, 0.02, 0.02)
end

Rough = MapEnvironmentModifier()
Rough.Key = "environment-modifier-terrain-rough"
Rough.Name = "Rough"
function Rough:Apply(environment)
	environment.Parameters.Terrain.ElevationLayer.Noise.Frequency = Vector3(0.015, 0.015, 0.015)
end

Rugged = MapEnvironmentModifier()
Rugged.Key = "environment-modifier-terrain-rugged"
Rugged.Name = "Rugged"
function Rugged:Apply(environment)
	environment.Parameters.Terrain.ElevationLayer.Noise.Frequency = Vector3(0.01, 0.01, 0.01)
end

Flat = MapEnvironmentModifier()
Flat.Key = "environment-modifier-terrain-flat"
Flat.Name = "Very Rough"
function Flat:Apply(environment)
	environment.Parameters.Terrain.ElevationLayer.Noise.Frequency = Vector3(0.0075, 0.0075, 0.0075)
end

VeryFlat = MapEnvironmentModifier()
VeryFlat.Key = "environment-modifier-terrain-very-flat"
VeryFlat.Name = "Very Flat"
function VeryFlat:Apply(environment)
	environment.Parameters.Terrain.ElevationLayer.Noise.Frequency = Vector3(0.005, 0.005, 0.005)
end

SteepPlateau = MapEnvironmentModifier()
SteepPlateau.Key = "environment-modifier-terrain-steep-plateau"
SteepPlateau.Name = "Steep Plateau"
function SteepPlateau:Apply(environment)
	environment.Parameters.Terrain.ElevationLayer.StandardDeviation = 
		0.25 * environment.Parameters.Terrain.ElevationLayer.StandardDeviation
end

Plateau = MapEnvironmentModifier()
Plateau.Key = "environment-modifier-terrain-plateau"
Plateau.Name = "Plateau"
function Plateau:Apply(environment)
	environment.Parameters.Terrain.ElevationLayer.StandardDeviation = 
		0.5 * environment.Parameters.Terrain.ElevationLayer.StandardDeviation
end

Rolling = MapEnvironmentModifier()
Rolling.Key = "environment-modifier-terrain-rolling"
Rolling.Name = "Rolling"
function Rolling:Apply(environment)
end

Smooth = MapEnvironmentModifier()
Smooth.Key = "environment-modifier-terrain-smooth"
Smooth.Name = "Smooth"
function Smooth:Apply(environment)
	environment.Parameters.Terrain.ElevationLayer.StandardDeviation = 
		2 * environment.Parameters.Terrain.ElevationLayer.StandardDeviation
end

VerySmooth = MapEnvironmentModifier()
VerySmooth.Key = "environment-modifier-terrain-very-smooth"
VerySmooth.Name = "Very Smooth"
function VerySmooth:Apply(environment)
	environment.Parameters.Terrain.ElevationLayer.StandardDeviation = 
		4 * environment.Parameters.Terrain.ElevationLayer.StandardDeviation
end


function Load()
	return { VeryRough, Rough, Rugged, Flat, VeryFlat, SteepPlateau, Plateau, Rolling, Smooth, VerySmooth }
end