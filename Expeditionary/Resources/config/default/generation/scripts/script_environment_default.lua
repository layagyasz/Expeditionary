luanet.load_assembly('Cardamom', 'Cardamom.Utils.Generators.Samplers')
luanet.load_assembly('Expeditionary', 'Expeditionary.Model')
luanet.load_assembly('OpenTK.Mathematics', 'OpenTK.Mathematics')

CityGenerator_Parameters=luanet.import_type('Expeditionary.Model.Mapping.Generator.CityGenerator+Parameters')
EdgeType=luanet.import_type('Expeditionary.Model.Mapping.EdgeType')
MapEnvironmentModifier=luanet.import_type('Expeditionary.Model.Mapping.Generator.MapEnvironmentModifier')
NormalSampler=luanet.import_type('Cardamom.Utils.Generators.Samplers.NormalSampler')
Quadratic=luanet.import_type('Expeditionary.Model.Quadratic')
StructureType=luanet.import_type('Expeditionary.Model.Mapping.StructureType')
TransportGenerator_Parameters=luanet.import_type('Expeditionary.Model.Mapping.Generator.TransportGenerator+Parameters')
Vector3=luanet.import_type('OpenTK.Mathematics.Vector3')

Default = MapEnvironmentModifier()
Default.Key = "environment-modifier-default"
Default.Name = "Default"
function Default:Apply(parameters)
	-- Basic
	parameters.Terrain.ElevationLevels = 5
	parameters.Terrain.LiquidLevel = 0.25
	parameters.Terrain.Stone = Vector3(1, 1, 1)
	parameters.Terrain.SoilCover = 0.9
	parameters.Terrain.BrushCover = 0.9
	parameters.Terrain.FoliageCover = 0.6
	parameters.Terrain.LiquidMoistureBonus = 0.2
	parameters.Terrain.Rivers = 100

	-- Soil
	-- Sand
	parameters.Terrain.SoilA.Weight = 1
	parameters.Terrain.SoilA.ElevationWeight = Quadratic(0, -1, 1)
	parameters.Terrain.SoilA.SlopeWeight = Quadratic(0, -1, 1)
	-- Clay
	parameters.Terrain.SoilB.Weight = 1
	-- Silt
	parameters.Terrain.SoilC.Weight = 1
	parameters.Terrain.SoilC.ElevationWeight = Quadratic(0, -1, 1)

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
	parameters.Cities:Add(mining)
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
	parameters.Cities:Add(farming)
	-- Commercial
	local commercial = CityGenerator_Parameters()
	commercial.Cores = 10
	commercial.Candidates = 200
	commercial.Type = StructureType.Commercial
	commercial.Size = NormalSampler(10, 5)
	parameters.Cities:Add(commercial)
	-- Residential
	local residential = CityGenerator_Parameters()
	residential.Cores = 40
	residential.Candidates = 200
	residential.Type = StructureType.Residential
	residential.Size = NormalSampler(20, 8)
	parameters.Cities:Add(residential)
	-- Industrial
	local industrial = CityGenerator_Parameters()
	industrial.Cores = 10
	industrial.Candidates = 200
	industrial.Type = StructureType.Industrial
	industrial.Size = NormalSampler(3, 1)
	industrial.RiverPenalty = Quadratic()
	parameters.Cities:Add(industrial)

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