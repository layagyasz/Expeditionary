luanet.load_assembly('Cardamom', 'Cardamom.Mathematics')
luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

Interval=luanet.import_type('Cardamom.Mathematics.Interval')
MapEnvironmentModifier=luanet.import_type('Expeditionary.Model.Mapping.MapEnvironmentModifier')

Desert = MapEnvironmentModifier()
Desert.Key = "environment-modifier-climate-desert"
Desert.Name = "Desert"
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

Arid = MapEnvironmentModifier()
Arid.Key = "environment-modifier-climate-arid"
Arid.Name = "Arid"
function Arid:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.SoilA.Weight = 1.5
	terrain.MoistureRange = Interval(0.0, 0.2)
end

SemiArid = MapEnvironmentModifier()
SemiArid.Key = "environment-modifier-climate-semiarid"
SemiArid.Name = "Semi Arid"
function SemiArid:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.MoistureRange = Interval(0.2, 0.4)
end

Average = MapEnvironmentModifier()
Average.Key = "environment-modifier-climate-average"
Average.Name = "Average"
function Average:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.MoistureRange = Interval(0.4, 0.6)
end

SemiWet = MapEnvironmentModifier()
SemiWet.Key = "environment-modifier-climate-semiwet"
SemiWet.Name = "Semi Wet"
function SemiWet:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.MoistureRange = Interval(0.6, 0.8)
end

Wet = MapEnvironmentModifier()
Wet.Key = "environment-modifier-climate-wet"
Wet.Name = "Wet"
function Wet:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.MoistureRange = Interval(0.8, 1.0)
end

Aquifer = MapEnvironmentModifier()
Aquifer.Key = "environment-modifier-climate-aquifer"
Aquifer.Name = "Aquifer"
function Aquifer:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.MoistureRange = Interval(0.0, 1.0)
end


Cold = MapEnvironmentModifier()
Cold.Key = "environment-modifier-climate-cold"
Cold.Name = "Cold"
function Cold:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.TemperatureRange = Interval(0.0, 0.2)
end

Cool = MapEnvironmentModifier()
Cool.Key = "environment-modifier-climate-cool"
Cool.Name = "Cool"
function Cool:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.TemperatureRange = Interval(0.2, 0.4)
end

Temperate = MapEnvironmentModifier()
Temperate.Key = "environment-modifier-climate-temperate"
Temperate.Name = "Temperate"
function Temperate:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.TemperatureRange = Interval(0.4, 0.6)
end

Warm = MapEnvironmentModifier()
Warm.Key = "environment-modifier-climate-warm"
Warm.Name = "Warm"
function Warm:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.TemperatureRange = Interval(0.6, 0.8)
end

Hot = MapEnvironmentModifier()
Hot.Key = "environment-modifier-climate-hot"
Hot.Name = "Hot"
function Hot:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.TemperatureRange = Interval(0.8, 1.0)
end

function Load()
	return { Desert, Arid, SemiArid, Average, SemiWet, Wet, Aquifer, Cold, Cool, Temperate, Warm, Hot }
end