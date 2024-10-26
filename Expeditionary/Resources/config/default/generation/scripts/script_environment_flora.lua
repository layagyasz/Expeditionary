luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

MapEnvironmentModifier=luanet.import_type('Expeditionary.Model.Mapping.MapEnvironmentModifier')

Dead = MapEnvironmentModifier()
Dead.Key = "environment-modifier-flora-dead"
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

Desert = MapEnvironmentModifier()
Desert.Key = "environment-modifier-flora-desert"
Desert.Name = "Desert"
function Desert:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.BrushCover = 0.1
	terrain.FoliageCover = 0
	terrain.RiverDensity = 0
	terrain.SoilA.Weight = 1
	terrain.SoilB.Weight = 0
	terrain.SoilC.Weight = 0
end

Nominal = MapEnvironmentModifier()
Nominal.Key = "environment-modifier-flora-nominal"
Nominal.Name = "Nominal"
function Nominal:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.BrushCover = 0.2
	terrain.FoliageCover = 0
	terrain.RiverDensity = 0
	terrain.SoilA.Weight = 1
	terrain.SoilB.Weight = 1
	terrain.SoilC.Weight = 0.5
end

Marginal = MapEnvironmentModifier()
Marginal.Key = "environment-modifier-flora-marginal"
Marginal.Name = "Marginal"
function Marginal:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.BrushCover = 0
	terrain.FoliageCover = 0
	terrain.RiverDensity = 0
	terrain.SoilA.Weight = 2
	terrain.SoilB.Weight = 1
	terrain.SoilC.Weight = 0.2
end

Habitable = MapEnvironmentModifier()
Habitable.Key = "environment-modifier-flora-habitable"
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


Lush = MapEnvironmentModifier()
Lush.Key = "environment-modifier-flora-lush"
Lush.Name = "Lush"
function Lush:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.BrushCover = 1
	terrain.FoliageCover = 0.8
	terrain.RiverDensity = 0.015
	terrain.SoilA.Weight = 0.5
	terrain.SoilB.Weight = 1
	terrain.SoilC.Weight = 2
end


function Load()
	return { Dead, Desert, Nominal, Marginal, Habitable, Lush  }
end