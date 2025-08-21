luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

MapEnvironmentTrait=luanet.import_type('Expeditionary.Model.Mapping.Environments.MapEnvironmentTrait')

Dead = MapEnvironmentTrait()
Dead.Key = "environment-trait-habitability-dead"
Dead.Name = "Dead"
function Dead:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.BrushCover = 0
	terrain.FoliageCover = 0
	terrain.RiverDensity = 0
	terrain.SoilA.Weight = 1
	terrain.SoilB.Weight = 0
	terrain.SoilC.Weight = 0
end

Nominal = MapEnvironmentTrait()
Nominal.Key = "environment-trait-habitability-nominal"
Nominal.Name = "Nominal"
function Nominal:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.BrushCover = 0.2
	terrain.FoliageCover = 0.15
	terrain.RiverDensity = 0
	terrain.SoilA.Weight = 1
	terrain.SoilB.Weight = 1
	terrain.SoilC.Weight = 0.5
end

Marginal = MapEnvironmentTrait()
Marginal.Key = "environment-trait-habitability-marginal"
Marginal.Name = "Marginal"
function Marginal:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.BrushCover = 0.1
	terrain.FoliageCover = 0.075
	terrain.RiverDensity = 0.025
	terrain.SoilA.Weight = 2
	terrain.SoilB.Weight = 1
	terrain.SoilC.Weight = 0.2
end

Habitable = MapEnvironmentTrait()
Habitable.Key = "environment-trait-habitability-habitable"
Habitable.Name = "Habitable"
function Habitable:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.BrushCover = 0.9
	terrain.FoliageCover = 0.7
	terrain.RiverDensity = 0.01
	terrain.SoilA.Weight = 1
	terrain.SoilB.Weight = 1
	terrain.SoilC.Weight = 1
end

Fertile = MapEnvironmentTrait()
Fertile.Key = "environment-trait-habitability-fertile"
Fertile.Name = "Fertile"
function Fertile:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.BrushCover = 0.9
	terrain.FoliageCover = 0.7
	terrain.RiverDensity = 0.01
	terrain.SoilA.Weight = 0.5
	terrain.SoilB.Weight = 1
	terrain.SoilC.Weight = 2
end

function Load()
	return { Dead, Nominal, Marginal, Habitable, Fertile  }
end