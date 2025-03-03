luanet.load_assembly('Expeditionary', 'Expeditionary.Model')
luanet.load_assembly('OpenTK.Mathematics', 'OpenTK.Mathematics')

Color4=luanet.import_type('OpenTK.Mathematics.Color4')
MapEnvironmentModifier=luanet.import_type('Expeditionary.Model.Mapping.MapEnvironmentModifier')
StaticColoring=luanet.import_type('Expeditionary.Model.Mapping.Appearance.IColoring+StaticColoring')
Vector3=luanet.import_type('OpenTK.Mathematics.Vector3')

Carbide = MapEnvironmentModifier()
Carbide.Key = "environment-modifier-composition-carbide"
Carbide.Name = "Carbide"
function Carbide:Apply(environment)
	local appearance = environment.Appearance;
	local parameters = environment.Parameters
	local terrain = parameters.Terrain

	terrain.Stone = Vector3(1, 0, 0)
	
	appearance.Stone.A = StaticColoring(Color4(0.13, 0.13, 0.13, 1))

	appearance.Soil.A = StaticColoring(Color4(0.35, 0.22, 0.14, 1))
	appearance.Soil.B = StaticColoring(Color4(0.36, 0.34, 0.29, 1))
	appearance.Soil.C = StaticColoring(Color4(0.23, 0.21, 0.21, 1))
end

Cupric = MapEnvironmentModifier()
Cupric.Key = "environment-modifier-composition-cupric"
Cupric.Name = "Cupric"
function Cupric:Apply(environment)
	local appearance = environment.Appearance;
	local parameters = environment.Parameters
	local terrain = parameters.Terrain

	terrain.Stone = Vector3(1, 0, 0)
	
	appearance.Stone.A = StaticColoring(Color4(0.2, 0.35, 0.2, 1))

	appearance.Soil.A = StaticColoring(Color4(0.31, 0.43, 0.31, 1))
	appearance.Soil.B = StaticColoring(Color4(0.25, 0.45, 0.25, 1))
	appearance.Soil.C = StaticColoring(Color4(0.18, 0.23, 0.18, 1))
end

Ferrous = MapEnvironmentModifier()
Ferrous.Key = "environment-modifier-composition-ferrous"
Ferrous.Name = "Ferrous"
function Ferrous:Apply(environment)
	local appearance = environment.Appearance;
	local parameters = environment.Parameters
	local terrain = parameters.Terrain

	terrain.Stone = Vector3(1, 1, 1)
	
	appearance.Stone.A = StaticColoring(Color4(0.65, 0.42, 0.33, 1))
	appearance.Stone.B = StaticColoring(Color4(0.13, 0.13, 0.13, 1))
	appearance.Stone.C = StaticColoring(Color4(0.74, 0.74, 0.74, 1))

	appearance.Soil.A = StaticColoring(Color4(0.65, 0.42, 0.33, 1))
	appearance.Soil.B = StaticColoring(Color4(0.51, 0.27, 0.27, 1))
	appearance.Soil.C = StaticColoring(Color4(0.23, 0.19, 0.18, 1))
end

SilicousFerrous = MapEnvironmentModifier()
SilicousFerrous.Key = "environment-modifier-composition-silicous-ferrous"
SilicousFerrous.Name = "Silicous Ferrous"
function SilicousFerrous:Apply(environment)
	local appearance = environment.Appearance;
	local parameters = environment.Parameters
	local terrain = parameters.Terrain

	terrain.Stone = Vector3(1, 0, 0)
	
	appearance.Stone.A = StaticColoring(Color4(0.65, 0.42, 0.33, 1))

	appearance.Soil.A = StaticColoring(Color4(0.85, 0.47, 0.35, 1))
	appearance.Soil.B = StaticColoring(Color4(0.51, 0.27, 0.27, 1))
	appearance.Soil.C = StaticColoring(Color4(0.23, 0.19, 0.18, 1))
end

SilicousMixed = MapEnvironmentModifier()
SilicousMixed.Key = "environment-modifier-composition-silicous-mixed"
SilicousMixed.Name = "Silicous Mixed"
function SilicousMixed:Apply(environment)
	local appearance = environment.Appearance;
	local parameters = environment.Parameters
	local terrain = parameters.Terrain

	terrain.Stone = Vector3(1, 1, 0)
	
	appearance.Stone.A = StaticColoring(Color4(0.48, 0.48, 0.48, 1))
	appearance.Stone.B = StaticColoring(Color4(0.65, 0.42, 0.33, 1))

	appearance.Soil.A = StaticColoring(Color4(0.85, 0.47, 0.35, 1))
	appearance.Soil.B = StaticColoring(Color4(0.85, 0.47, 0.35, 1))
	appearance.Soil.C = StaticColoring(Color4(0.23, 0.19, 0.18, 1))
end

SilicousTelluric = MapEnvironmentModifier()
SilicousTelluric.Key = "environment-modifier-composition-silicous-telluric"
SilicousTelluric.Name = "Silicous Telluric"
function SilicousTelluric:Apply(environment)
	local appearance = environment.Appearance;
	local parameters = environment.Parameters
	local terrain = parameters.Terrain

	terrain.Stone = Vector3(1, 0, 0)
	
	appearance.Stone.A = StaticColoring(Color4(0.48, 0.48, 0.48, 1))

	appearance.Soil.A = StaticColoring(Color4(0.74, 0.74, 0.74, 1))
	appearance.Soil.B = StaticColoring(Color4(0.36, 0.34, 0.29, 1))
	appearance.Soil.C = StaticColoring(Color4(0.23, 0.21, 0.21, 1))
end

SilicousTerran = MapEnvironmentModifier()
SilicousTerran.Key = "environment-modifier-composition-silicous-terran"
SilicousTerran.Name = "Silicous Terran"
function SilicousTerran:Apply(environment)
	local appearance = environment.Appearance;
	local parameters = environment.Parameters
	local terrain = parameters.Terrain

	terrain.Stone = Vector3(1, 1, 1)
	
	appearance.Stone.A = StaticColoring(Color4(0.48, 0.35, 0.19, 1))
	appearance.Stone.B = StaticColoring(Color4(0.78, 0.78, 0.78, 1))
	appearance.Stone.C = StaticColoring(Color4(0.89, 0.68, 0.61, 1))

	appearance.Soil.A = StaticColoring(Color4(0.96, 0.83, 0.68, 1))
	appearance.Soil.B = StaticColoring(Color4(0.58, 0.48, 0.24, 1))
	appearance.Soil.C = StaticColoring(Color4(0.23, 0.19, 0.18, 1))
end

Sulfurous = MapEnvironmentModifier()
Sulfurous.Key = "environment-modifier-composition-sulfurous"
Sulfurous.Name = "Sulfurous"
function Sulfurous:Apply(environment)
	local appearance = environment.Appearance;
	local parameters = environment.Parameters
	local terrain = parameters.Terrain

	terrain.Stone = Vector3(1, 0, 0)
	
	appearance.Stone.A = StaticColoring(Color4(0.68, 0.49, 0.25, 1))

	appearance.Soil.A = StaticColoring(Color4(0.89, 0.8, 0.59, 1))
	appearance.Soil.B = StaticColoring(Color4(0.58, 0.48, 0.24, 1))
	appearance.Soil.C = StaticColoring(Color4(0.23, 0.23, 0.18, 1))
end

Volcanic = MapEnvironmentModifier()
Volcanic.Key = "environment-modifier-composition-volcanic"
Volcanic.Name = "Volcanic"
function Volcanic:Apply(environment)
	local appearance = environment.Appearance;
	local parameters = environment.Parameters
	local terrain = parameters.Terrain

	terrain.Stone.C = 1
	appearance.Soil.C = StaticColoring(Color4(0.23, 0.23, 0.23, 1))
end

function Load()
	return {  
		Carbide,
		Cupric, 
		Ferrous, 
		SilicousFerrous,
		SilicousMixed,
		SilicousTelluric, 
		SilicousTerran, 
		Sulfurous, 
		Volcanic 
	}
end