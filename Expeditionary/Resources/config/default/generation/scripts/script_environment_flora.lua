luanet.load_assembly('Expeditionary', 'Expeditionary.Model')
luanet.load_assembly('OpenTK.Mathematics', 'OpenTK.Mathematics')

Color4=luanet.import_type('OpenTK.Mathematics.Color4')
MapEnvironmentModifier=luanet.import_type('Expeditionary.Model.Mapping.MapEnvironmentModifier')
SolarOutputOffsetColoring=luanet.import_type('Expeditionary.Model.Mapping.Appearance.IColoring+SolarOutputOffsetColoring')
StaticColoring=luanet.import_type('Expeditionary.Model.Mapping.Appearance.IColoring+StaticColoring')

Native = MapEnvironmentModifier()
Native.Key = "environment-modifier-flora-native"
Native.Name = "Native"
function Native:Apply(environment)
	local appearance = environment.Appearance;

	appearance.Brush.HotDry = SolarOutputOffsetColoring(Color4(-0.33, 0.25, 0.9, 1))
	appearance.Brush.HotWet = SolarOutputOffsetColoring(Color4(-0.13, 0.67, 0.4, 1))
	appearance.Brush.ColdDry = SolarOutputOffsetColoring(Color4(-0.34, 0.32, 0.7, 1))
	appearance.Brush.ColdWet = SolarOutputOffsetColoring(Color4(0.03, 0.2, 0.7, 1))

	appearance.Foliage.HotDry = SolarOutputOffsetColoring(Color4(-0.32, 0.45, 0.5, 1))
	appearance.Foliage.HotWet = SolarOutputOffsetColoring(Color4(-0.02, 0.45, 0.17, 1))
	appearance.Foliage.ColdDry = SolarOutputOffsetColoring(Color4(-0.08, 0.64, 0.5, 1))
	appearance.Foliage.ColdWet = SolarOutputOffsetColoring(Color4(0.03, 0.8, 0.3, 1))
end

Terran = MapEnvironmentModifier()
Terran.Key = "environment-modifier-flora-terran"
Terran.Name = "Terran"
function Terran:Apply(environment)
	local appearance = environment.Appearance;

	appearance.Brush.HotDry = StaticColoring(Color4(0.68, 0.64, 0.58, 1))
	appearance.Brush.HotWet = StaticColoring(Color4(0.21, 0.3, 0.19, 1))
	appearance.Brush.ColdDry = StaticColoring(Color4(0.53, 0.48, 0.43, 1))
	appearance.Brush.ColdWet = StaticColoring(Color4(0.47, 0.53, 0.51, 1))
	
	appearance.Foliage.HotDry = StaticColoring(Color4(0.38, 0.35, 0.28, 1))
	appearance.Foliage.HotWet = StaticColoring(Color4(0.09, 0.13, 0.11, 1))
	appearance.Foliage.ColdDry = StaticColoring(Color4(0.24, 0.38, 0.25, 1))
	appearance.Foliage.ColdWet = StaticColoring(Color4(0.13, 0.22, 0.2, 1))
end

Exotic = MapEnvironmentModifier()
Exotic.Key = "environment-modifier-flora-exotic"
Exotic.Name = "Exotic"
function Exotic:Apply(environment)
	local appearance = environment.Appearance;

	appearance.Brush.HotDry = StaticColoring(Color4(0.68, 0.68, 0.68, 1))
	appearance.Brush.HotWet = StaticColoring(Color4(0.55, 0.55, 0.55, 1))
	appearance.Brush.ColdDry = StaticColoring(Color4(0.68, 0.68, 0.68, 1))
	appearance.Brush.ColdWet = StaticColoring(Color4(0.55, 0.55, 0.55, 1))
	
	appearance.Foliage.HotDry = StaticColoring(Color4(0.51, 0.42, 0.6, 1))
	appearance.Foliage.HotWet = StaticColoring(Color4(0.22, 0.15, 0.3, 1))
	appearance.Foliage.ColdDry = StaticColoring(Color4(0.56, 0.51, 0.6, 1))
	appearance.Foliage.ColdWet = StaticColoring(Color4(0.25, 0.21, 0.3, 1))
end

Fungal = MapEnvironmentModifier()
Fungal.Key = "environment-modifier-flora-fungal"
Fungal.Name = "Fungal"
function Fungal:Apply(environment)
	local appearance = environment.Appearance;

	appearance.Brush.HotDry = SolarOutputOffsetColoring(Color4(-0.33, 0.25, 0.9, 1))
	appearance.Brush.HotWet = SolarOutputOffsetColoring(Color4(-0.13, 0.67, 0.4, 1))
	appearance.Brush.ColdDry = SolarOutputOffsetColoring(Color4(-0.34, 0.32, 0.7, 1))
	appearance.Brush.ColdWet = SolarOutputOffsetColoring(Color4(0.03, 0.2, 0.7, 1))
	
	appearance.Foliage.HotDry = StaticColoring(Color4(0.5, 0.36, 0.3, 1))
	appearance.Foliage.HotWet = StaticColoring(Color4(0.27, 0.22, 0.18, 1))
	appearance.Foliage.ColdDry = StaticColoring(Color4(0.95, 0.9, 0.86, 1))
	appearance.Foliage.ColdWet = StaticColoring(Color4(0.7, 0.14, 0.14, 1))
end

Crystalline = MapEnvironmentModifier()
Crystalline.Key = "environment-modifier-flora-crystalline"
Crystalline.Name = "Fungal"
function Crystalline:Apply(environment)
	local appearance = environment.Appearance;

	appearance.Brush.HotDry = StaticColoring(Color4(0.68, 0.68, 0.68, 1))
	appearance.Brush.HotWet = StaticColoring(Color4(0.68, 0.68, 0.68, 1))
	appearance.Brush.ColdDry = StaticColoring(Color4(0.68, 0.68, 0.68, 1))
	appearance.Brush.ColdWet = StaticColoring(Color4(0.68, 0.68, 0.68, 1))
	
	appearance.Foliage.HotDry = StaticColoring(Color4(0.95, 0.95, 0.95, 1))
	appearance.Foliage.HotWet = StaticColoring(Color4(0.95, 0.95, 0.95, 1))
	appearance.Foliage.ColdDry = StaticColoring(Color4(0.95, 0.95, 0.95, 1))
	appearance.Foliage.ColdWet = StaticColoring(Color4(0.95, 0.95, 0.95, 1))
end

function Load()
	return { Native, Terran, Exotic, Fungal, Crystalline }
end