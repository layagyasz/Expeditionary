luanet.load_assembly('Cardamom', 'Cardamom.Mathematics')
luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

Interval=luanet.import_type('Cardamom.Mathematics.Interval')
MapEnvironmentTrait=luanet.import_type('Expeditionary.Model.Mapping.Environments.MapEnvironmentTrait')

Desert = MapEnvironmentTrait()
Desert.Key = "environment-trait-climate-desert"
function Desert:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.BrushCover = terrain.BrushCover * 0.1
	terrain.FoliageCover = 0
	terrain.RiverDensity = 0
	terrain.SoilA.Weight = 1
	terrain.SoilB.Weight = 0
	terrain.SoilC.Weight = 0
	terrain.MoistureRange = Interval(0, 0.2)
end

Arid = MapEnvironmentTrait()
Arid.Key = "environment-trait-climate-arid"
function Arid:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.SoilA.Weight = 1.5
	terrain.MoistureRange = Interval(0.0, 0.2)
end

SemiArid = MapEnvironmentTrait()
SemiArid.Key = "environment-trait-climate-semiarid"
function SemiArid:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.MoistureRange = Interval(0.2, 0.4)
end

Average = MapEnvironmentTrait()
Average.Key = "environment-trait-climate-average"
function Average:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.MoistureRange = Interval(0.4, 0.6)
end

SemiWet = MapEnvironmentTrait()
SemiWet.Key = "environment-trait-climate-semiwet"
function SemiWet:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.MoistureRange = Interval(0.6, 0.8)
end

Wet = MapEnvironmentTrait()
Wet.Key = "environment-trait-climate-wet"
function Wet:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.MoistureRange = Interval(0.8, 1.0)
end

Aquifer = MapEnvironmentTrait()
Aquifer.Key = "environment-trait-climate-aquifer"
function Aquifer:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.MoistureRange = Interval(0.0, 1.0)
end

Cold = MapEnvironmentTrait()
Cold.Key = "environment-trait-climate-cold"
function Cold:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.TemperatureRange = Interval(0.0, 0.2)
	terrain.GroundCoverCover = 0.2
end

Cool = MapEnvironmentTrait()
Cool.Key = "environment-trait-climate-cool"
function Cool:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.TemperatureRange = Interval(0.2, 0.4)
end

Temperate = MapEnvironmentTrait()
Temperate.Key = "environment-trait-climate-temperate"
function Temperate:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.TemperatureRange = Interval(0.4, 0.6)
end

Warm = MapEnvironmentTrait()
Warm.Key = "environment-trait-climate-warm"
function Warm:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.TemperatureRange = Interval(0.6, 0.8)
end

Hot = MapEnvironmentTrait()
Hot.Key = "environment-trait-climate-hot"
function Hot:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.TemperatureRange = Interval(0.8, 1.0)
end

function Load()
	return { Desert, Arid, SemiArid, Average, SemiWet, Wet, Aquifer, Arctic, Cold, Cool, Temperate, Warm, Hot }
end