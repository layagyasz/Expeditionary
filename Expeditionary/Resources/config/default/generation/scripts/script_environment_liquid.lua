luanet.load_assembly('Expeditionary', 'Expeditionary.Model')
luanet.load_assembly('OpenTK.Mathematics', 'OpenTK.Mathematics')

Color4=luanet.import_type('OpenTK.Mathematics.Color4')
MapEnvironmentModifier=luanet.import_type('Expeditionary.Model.Mapping.MapEnvironmentModifier')
StaticColoring=luanet.import_type('Expeditionary.Model.Mapping.Appearance.IColoring+StaticColoring')

Carbide = MapEnvironmentModifier()
Carbide.Key = "environment-modifier-liquid-carbide"
Carbide.Name = "Carbide"
function Carbide:Apply(environment)
	environment.Parameters.Cities.LiquidAffinity = 0
	environment.Parameters.Terrain.LiquidMoistureBonus = 0
	environment.Appearance.Liquid = StaticColoring(Color4(0.29, 0.19, 0.12, 1))
end

Ferrous = MapEnvironmentModifier()
Ferrous.Key = "environment-modifier-liquid-ferrous"
Ferrous.Name = "Ferrous"
function Ferrous:Apply(environment)
	environment.Parameters.Cities.LiquidAffinity = 0
	environment.Parameters.Terrain.LiquidMoistureBonus = 0
	environment.Appearance.Liquid = StaticColoring(Color4(0.38, 0.16, 0.07, 1))
end

Lava = MapEnvironmentModifier()
Lava.Key = "environment-modifier-liquid-lava"
Lava.Name = "Lava"
function Lava:Apply(environment)
	environment.Parameters.Cities.LiquidAffinity = -1
	environment.Parameters.Terrain.LiquidMoistureBonus = -1
	environment.Appearance.Liquid = StaticColoring(Color4(0.83, 0.29, 0.15, 1))
end

Sulfurous = MapEnvironmentModifier()
Sulfurous.Key = "environment-modifier-liquid-sulfurous"
Sulfurous.Name = "Sulfurous"
function Sulfurous:Apply(environment)
	environment.Parameters.Cities.LiquidAffinity = 0
	environment.Parameters.Terrain.LiquidMoistureBonus = 0
	environment.Appearance.Liquid = StaticColoring(Color4(0.79, 0.75, 0.45, 1))
end

Water = MapEnvironmentModifier()
Water.Key = "environment-modifier-liquid-water"
Water.Name = "Water"
function Water:Apply(environment)
	environment.Parameters.Cities.LiquidAffinity = 1
	environment.Parameters.Terrain.LiquidMoistureBonus = 0.2
	environment.Appearance.Liquid = StaticColoring(Color4(0.1, 0.22, 0.33, 1))
end

function Load()
	return { Carbide, Ferrous, Lava, Sulfurous, Water }
end