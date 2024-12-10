luanet.load_assembly('Cardamom', 'Cardamom.Utils.Generators.Samplers')
luanet.load_assembly('Expeditionary', 'Expeditionary.Model')
luanet.load_assembly('OpenTK.Mathematics', 'OpenTK.Mathematics')

CityGenerator_Parameters=luanet.import_type('Expeditionary.Model.Mapping.Generator.CityGenerator+LayerParameters')
Color4=luanet.import_type('OpenTK.Mathematics.Color4')
EdgeType=luanet.import_type('Expeditionary.Model.Mapping.EdgeType')
MapEnvironmentModifier=luanet.import_type('Expeditionary.Model.Mapping.MapEnvironmentModifier')
NormalSampler=luanet.import_type('Cardamom.Utils.Generators.Samplers.NormalSampler')
Quadratic=luanet.import_type('Expeditionary.Model.Quadratic')
StaticColoring=luanet.import_type('Expeditionary.Model.Mapping.Appearance.IColoring+StaticColoring')
StructureType=luanet.import_type('Expeditionary.Model.Mapping.StructureType')
TransportGenerator_Parameters=luanet.import_type('Expeditionary.Model.Mapping.Generator.TransportGenerator+Parameters')
Vector3=luanet.import_type('OpenTK.Mathematics.Vector3')

Default = MapEnvironmentModifier()
Default.Key = "environment-modifier-default"
Default.Name = "Default"
function Default:Apply(environment)
	local parameters = environment.Parameters
	local terrain = parameters.Terrain

	-- Basic
	terrain.ElevationLayer.Noise.Frequency = Vector3(0.01, 0.01, 0.01)

	terrain.StoneLayer.Noise.Frequency = Vector3(0.02, 0.02, 0.02)
	terrain.StoneLayer.StandardDeviation = 0.2
	terrain.StoneLayer.Transform = Quadratic(0, 0.5, 0.5)

	terrain.SoilLayer.Noise.Frequency = Vector3(0.02, 0.02, 0.02)
	terrain.SoilLayer.StandardDeviation = 0.2
	terrain.SoilLayer.Transform = Quadratic(0, 0.5, 0.5)

	terrain.SoilCoverLayer.Noise.Frequency = Vector3(0.05, 0.05, 0.05)
	terrain.SoilCoverLayer.StandardDeviation = 0.2
	terrain.SoilCoverLayer.Transform = Quadratic(0, 0.5, 0.5)

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

	-- Habitation
	-- Mining
	local mining = CityGenerator_Parameters()
	mining.Cores = 5
	mining.Candidates = 100
	mining.Type = StructureType.Mining
	mining.Level = 1
	mining.Size = NormalSampler(3, 1)
	mining.RiverPenalty = Quadratic()
	mining.CoastPenalty = Quadratic()
	mining.SlopePenalty = Quadratic(0, -1, 1)
	mining.ElevationPenalty = Quadratic(0, -1, 1)
	parameters.Cities.Layers:Add(mining)
	-- Farming
	local farming = CityGenerator_Parameters()
	farming.Cores = 40
	farming.Candidates = 200
	farming.Type = StructureType.Agricultural
	farming.Size = NormalSampler(40, 20)
	farming.RiverPenalty = Quadratic(0, -2, 2)
	farming.CoastPenalty = Quadratic()
	farming.SiltPenalty = Quadratic(0, -2, 2)
	farming.MoisturePenalty = Quadratic(0, -1, 1)
	parameters.Cities.Layers:Add(farming)
	-- Commercial
	local commercial = CityGenerator_Parameters()
	commercial.Cores = 10
	commercial.Candidates = 200
	commercial.Type = StructureType.Commercial
	commercial.Size = NormalSampler(10, 5)
	parameters.Cities.Layers:Add(commercial)
	-- Residential
	local residential = CityGenerator_Parameters()
	residential.Cores = 40
	residential.Candidates = 200
	residential.Type = StructureType.Residential
	residential.Size = NormalSampler(20, 8)
	parameters.Cities.Layers:Add(residential)
	-- Industrial
	local industrial = CityGenerator_Parameters()
	industrial.Cores = 10
	industrial.Candidates = 200
	industrial.Type = StructureType.Industrial
	industrial.Size = NormalSampler(3, 1)
	industrial.RiverPenalty = Quadratic()
	parameters.Cities.Layers:Add(industrial)

	-- Roads
	local roads = TransportGenerator_Parameters()
	roads.Type = EdgeType.Road
	roads.Level = 1
	roads.SupportedStructures:Add(StructureType.Commercial, 1)
	roads.SupportedStructures:Add(StructureType.Residential, 1)
	roads.SupportedStructures:Add(StructureType.Mining, 1)
	roads.SupportedStructures:Add(StructureType.Industrial, 1)
	parameters.Transport:Add(roads)
end

function Load()
	return { Default }
end