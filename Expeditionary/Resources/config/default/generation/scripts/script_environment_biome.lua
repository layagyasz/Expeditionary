luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

MapEnvironmentTrait=luanet.import_type('Expeditionary.Model.Mapping.Environments.MapEnvironmentTrait')

Forest = MapEnvironmentTrait()
Forest.Key = "environment-trait-biome-forest"
Forest.Name = "Forest"
function Forest:Apply(environment)
end

Lush = MapEnvironmentTrait()
Lush.Key = "environment-trait-biome-lush"
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

Field = MapEnvironmentTrait()
Field.Key = "environment-trait-biome-field"
Field.Name = "Field"
function Field:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.FoliageCover = terrain.FoliageCover * 0.5
end

function Load()
	return { Forest, Lush, Field }
end