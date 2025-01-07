luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

MapEnvironmentModifier=luanet.import_type('Expeditionary.Model.Mapping.MapEnvironmentModifier')

Forest = MapEnvironmentModifier()
Forest.Key = "environment-modifier-biome-forest"
Forest.Name = "Forest"
function Forest:Apply(environment)
end

Lush = MapEnvironmentModifier()
Lush.Key = "environment-modifier-biome-lush"
Lush.Name = "Lush"
function Lush:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.BrushCover = 1
	terrain.FoliageCover = terrain.FoliageCover * 1.2
	terrain.RiverDensity = terrain.RiverDensity * 1.5
	terrain.SoilA.Weight = terrain.SoilA.Weight * 0.5
	terrain.SoilB.Weight = terrain.SoilB.Weight * 1
	terrain.SoilC.Weight = terrain.SoilC.Weight * 2
end

Field = MapEnvironmentModifier()
Field.Key = "environment-modifier-biome-field"
Field.Name = "Field"
function Field:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.FoliageCover = terrain.FoliageCover * 0.5
end

function Load()
	return { Forest, Lush, Field }
end