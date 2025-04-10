luanet.load_assembly('Expeditionary', 'Expeditionary.Model')
luanet.load_assembly('OpenTK.Mathematics', 'OpenTK.Mathematics')

MapEnvironmentModifier=luanet.import_type('Expeditionary.Model.Mapping.MapEnvironmentModifier')
Quadratic=luanet.import_type('Expeditionary.Model.Quadratic')
Vector3=luanet.import_type('OpenTK.Mathematics.Vector3')

Default = MapEnvironmentModifier()
Default.Key = "environment-modifier-default"
Default.Name = "Default"
function Default:Apply(environment)
	local parameters = environment.Parameters
	local terrain = parameters.Terrain

	terrain.BrushRequireSoil = true
	terrain.FoliageRequireSoil = true

	-- Basic
	terrain.StoneLayer.Noise.Frequency = Vector3(0.02, 0.02, 0.02)
	terrain.StoneLayer.StandardDeviation = 0.2
	terrain.StoneLayer.Transform = Quadratic(0, 0.5, 0.5)

	terrain.SoilLayer.Noise.Frequency = Vector3(0.02, 0.02, 0.02)
	terrain.SoilLayer.StandardDeviation = 0.2
	terrain.SoilLayer.Transform = Quadratic(0, 0.5, 0.5)

	terrain.SoilCoverLayer.Noise.Frequency = Vector3(0.05, 0.05, 0.05)
	terrain.SoilCoverLayer.StandardDeviation = 0.2
	terrain.SoilCoverLayer.Transform = Quadratic(0, 0.5, 0.5)

	terrain.GroundCoverLayer.Noise.Frequency = Vector3(0.05, 0.05, 0.05)
	terrain.GroundCoverLayer.StandardDeviation = 0.2
	terrain.GroundCoverLayer.Transform = Quadratic(0, 0.5, 0.5)

	terrain.TemperatureLayer.Noise.Frequency = Vector3(0.02, 0.02, 0.02)
	terrain.TemperatureLayer.StandardDeviation = 0.2
	terrain.TemperatureLayer.Transform = Quadratic(0, 0.5, 0.5)

	terrain.MoistureLayer.Noise.Frequency = Vector3(0.02, 0.02, 0.02)
	terrain.MoistureLayer.StandardDeviation = 0.2
	terrain.MoistureLayer.Transform = Quadratic(0, 0.5, 0.5)

	terrain.BrushLayer.Noise.Frequency = Vector3(0.05, 0.05, 0.05)
	terrain.BrushLayer.StandardDeviation = 0.2
	terrain.BrushLayer.Transform = Quadratic(0, 0.5, 0.5)

	terrain.FoliageLayer.Noise.Frequency = Vector3(0.2, 0.2, 0.2)
	terrain.FoliageLayer.StandardDeviation = 0.2
	terrain.FoliageLayer.Transform = Quadratic(0, 0.5, 0.5)

	terrain.ElevationLevels = 5

	-- Soil
	terrain.SoilA.ElevationWeight = Quadratic(0, -1, 1)
	terrain.SoilA.SlopeWeight = Quadratic(0, -1, 1)
	terrain.SoilC.ElevationWeight = Quadratic(0, -1, 1)
end

function Load()
	return { Default }
end