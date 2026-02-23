luanet.load_assembly('Cardamom', 'Cardamom.Utils.Generators.Samplers')
luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

CityGenerator_Parameters=luanet.import_type('Expeditionary.Model.Mapping.Generator.CityGenerator+LayerParameters')
EdgeType=luanet.import_type('Expeditionary.Model.Mapping.EdgeType')
MapEnvironmentTrait=luanet.import_type('Expeditionary.Model.Mapping.Environments.MapEnvironmentTrait')
NormalSampler=luanet.import_type('Cardamom.Utils.Generators.Samplers.NormalSampler')
Quadratic=luanet.import_type('Expeditionary.Model.Quadratic')
StructureType=luanet.import_type('Expeditionary.Model.Mapping.StructureType')
TransportGenerator_Parameters=luanet.import_type('Expeditionary.Model.Mapping.Generator.TransportGenerator+Parameters')
TransportGenerator_StructurePredicate=luanet.import_type('Expeditionary.Model.Mapping.Generator.TransportGenerator+StructurePredicate')


Habitation0 = MapEnvironmentTrait()
Habitation0.Key = "environment-trait-structure-habitation-0"
Habitation0.Name = "Uninhabited"
function Habitation0:Apply(environment)
end

Habitation1 = MapEnvironmentTrait()
Habitation1.Key = "environment-trait-structure-habitation-1"
Habitation1.Name = "Villages"
function Habitation1:Apply(environment)
	local parameters = environment.Parameters

	local commercial = CityGenerator_Parameters()
	commercial.CoreDensity = 0.001
	commercial.CandidateDensity = 0.02
	commercial.Type = StructureType.Commercial
	commercial.Size = NormalSampler(2, 1)
	parameters.Cities.Layers:Add(commercial)

	local residential = CityGenerator_Parameters()
	residential.CoreDensity = 0.004
	residential.CandidateDensity = 0.02
	residential.Type = StructureType.Residential
	residential.Size = NormalSampler(4, 2)
	parameters.Cities.Layers:Add(residential)

	local industrial = CityGenerator_Parameters()
	industrial.CoreDensity = 0.001
	industrial.CandidateDensity = 0.02
	industrial.Type = StructureType.Industrial
	industrial.Size = NormalSampler(1, 1)
	industrial.RiverPenalty = Quadratic()
	parameters.Cities.Layers:Add(industrial)
end

Habitation2 = MapEnvironmentTrait()
Habitation2.Key = "environment-trait-structure-habitation-2"
Habitation2.Name = "Small Towns"
function Habitation2:Apply(environment)
	local parameters = environment.Parameters

	local commercial = CityGenerator_Parameters()
	commercial.CoreDensity = 0.001
	commercial.CandidateDensity = 0.02
	commercial.Type = StructureType.Commercial
	commercial.Size = NormalSampler(4, 2)
	parameters.Cities.Layers:Add(commercial)

	local residential = CityGenerator_Parameters()
	residential.CoreDensity = 0.004
	residential.CandidateDensity = 0.02
	residential.Type = StructureType.Residential
	residential.Size = NormalSampler(8, 4)
	parameters.Cities.Layers:Add(residential)

	local industrial = CityGenerator_Parameters()
	industrial.CoreDensity = 0.001
	industrial.CandidateDensity = 0.02
	industrial.Type = StructureType.Industrial
	industrial.Size = NormalSampler(2, 1)
	industrial.RiverPenalty = Quadratic()
	parameters.Cities.Layers:Add(industrial)
end

Habitation3 = MapEnvironmentTrait()
Habitation3.Key = "environment-trait-structure-habitation-3"
Habitation3.Name = "Big Towns"
function Habitation3:Apply(environment)
	local parameters = environment.Parameters

	local commercial = CityGenerator_Parameters()
	commercial.CoreDensity = 0.001
	commercial.CandidateDensity = 0.02
	commercial.Type = StructureType.Commercial
	commercial.Size = NormalSampler(16, 8)
	parameters.Cities.Layers:Add(commercial)

	local residential = CityGenerator_Parameters()
	residential.CoreDensity = 0.004
	residential.CandidateDensity = 0.02
	residential.Type = StructureType.Residential
	residential.Size = NormalSampler(32, 16)
	parameters.Cities.Layers:Add(residential)

	local industrial = CityGenerator_Parameters()
	industrial.CoreDensity = 0.001
	industrial.CandidateDensity = 0.02
	industrial.Type = StructureType.Industrial
	industrial.Size = NormalSampler(8, 4)
	industrial.RiverPenalty = Quadratic()
	parameters.Cities.Layers:Add(industrial)
end

Habitation4 = MapEnvironmentTrait()
Habitation4.Key = "environment-trait-structure-habitation-4"
Habitation4.Name = "City"
function Habitation4:Apply(environment)
	local parameters = environment.Parameters

	local commercial = CityGenerator_Parameters()
	commercial.CoreDensity = 0.004
	commercial.CandidateDensity = 0.02
	commercial.Type = StructureType.Commercial
	commercial.Size = NormalSampler(16, 8)
	parameters.Cities.Layers:Add(commercial)

	local residential = CityGenerator_Parameters()
	residential.CoreDensity = 0.016
	residential.CandidateDensity = 0.032
	residential.Type = StructureType.Residential
	residential.Size = NormalSampler(32, 16)
	parameters.Cities.Layers:Add(residential)

	local industrial = CityGenerator_Parameters()
	industrial.CoreDensity = 0.004
	industrial.CandidateDensity = 0.02
	industrial.Type = StructureType.Industrial
	industrial.Size = NormalSampler(8, 4)
	industrial.RiverPenalty = Quadratic()
	parameters.Cities.Layers:Add(industrial)
end

Habitation5 = MapEnvironmentTrait()
Habitation5.Key = "environment-trait-structure-habitation-5"
Habitation5.Name = "Metro"
function Habitation5:Apply(environment)
	local parameters = environment.Parameters

	local commercial = CityGenerator_Parameters()
	commercial.CoreDensity = 0.008
	commercial.CandidateDensity = 0.02
	commercial.Type = StructureType.Commercial
	commercial.Size = NormalSampler(16, 8)
	parameters.Cities.Layers:Add(commercial)

	local residential = CityGenerator_Parameters()
	residential.CoreDensity = 0.032
	residential.CandidateDensity = 0.064
	residential.Type = StructureType.Residential
	residential.Size = NormalSampler(32, 16)
	parameters.Cities.Layers:Add(residential)

	local industrial = CityGenerator_Parameters()
	industrial.CoreDensity = 0.008
	industrial.CandidateDensity = 0.02
	industrial.Type = StructureType.Industrial
	industrial.Size = NormalSampler(8, 4)
	industrial.RiverPenalty = Quadratic()
	parameters.Cities.Layers:Add(industrial)
end

Farm0 = MapEnvironmentTrait()
Farm0.Key = "environment-trait-structure-farm-0"
Farm0.Name = "Unfarmed"
function Farm0:Apply(environment)
end

Farm1 = MapEnvironmentTrait()
Farm1.Key = "environment-trait-structure-farm-1"
Farm1.Name = "Sparsely Farmed"
function Farm1:Apply(environment)
	local parameters = environment.Parameters

	local farming = CityGenerator_Parameters()
	farming.CoreDensity = 0.004
	farming.CandidateDensity = 0.02
	farming.Type = StructureType.Agricultural
	farming.Size = NormalSampler(20, 10)
	farming.RiverPenalty = Quadratic(0, -2, 2)
	farming.CoastPenalty = Quadratic()
	farming.SiltPenalty = Quadratic(0, -2, 2)
	farming.MoisturePenalty = Quadratic(0, -1, 1)
	parameters.Cities.Layers:Add(farming)
end

Farm2 = MapEnvironmentTrait()
Farm2.Key = "environment-trait-structure-farm-2"
Farm2.Name = "Moderately Farmed"
function Farm2:Apply(environment)
	local parameters = environment.Parameters

	local farming = CityGenerator_Parameters()
	farming.CoreDensity = 0.004
	farming.CandidateDensity = 0.02
	farming.Type = StructureType.Agricultural
	farming.Size = NormalSampler(40, 20)
	farming.RiverPenalty = Quadratic(0, -2, 2)
	farming.CoastPenalty = Quadratic()
	farming.SiltPenalty = Quadratic(0, -2, 2)
	farming.MoisturePenalty = Quadratic(0, -1, 1)
	parameters.Cities.Layers:Add(farming)
end

Farm3 = MapEnvironmentTrait()
Farm3.Key = "environment-trait-structure-farm-3"
Farm3.Name = "Densely Farmed"
function Farm3:Apply(environment)
	local parameters = environment.Parameters

	local farming = CityGenerator_Parameters()
	farming.CoreDensity = 0.008
	farming.CandidateDensity = 0.02
	farming.Type = StructureType.Agricultural
	farming.Size = NormalSampler(40, 20)
	farming.RiverPenalty = Quadratic(0, -2, 2)
	farming.CoastPenalty = Quadratic()
	farming.SiltPenalty = Quadratic(0, -2, 2)
	farming.MoisturePenalty = Quadratic(0, -1, 1)
	parameters.Cities.Layers:Add(farming)
end

Mine0 = MapEnvironmentTrait()
Mine0.Key = "environment-trait-structure-mine-0"
Mine0.Name = "Unmined"
function Mine0:Apply(environment)
end

Mine1 = MapEnvironmentTrait()
Mine1.Key = "environment-trait-structure-mine-1"
Mine1.Name = "Sparsely Mined"
function Mine1:Apply(environment)
	local parameters = environment.Parameters

	local mining = CityGenerator_Parameters()
	mining.CoreDensity = 0.0004
	mining.CandidateDensity = 0.01
	mining.Type = StructureType.Mining
	mining.Level = 1
	mining.Size = NormalSampler(2, 1)
	mining.RiverPenalty = Quadratic()
	mining.CoastPenalty = Quadratic()
	mining.SlopePenalty = Quadratic(0, -1, 1)
	mining.ElevationPenalty = Quadratic(0, -1, 1)
	parameters.Cities.Layers:Add(mining)
end

Mine2 = MapEnvironmentTrait()
Mine2.Key = "environment-trait-structure-mine-2"
Mine2.Name = "Moderately Mined"
function Mine2:Apply(environment)
	local parameters = environment.Parameters

	local mining = CityGenerator_Parameters()
	mining.CoreDensity = 0.0006
	mining.CandidateDensity = 0.01
	mining.Type = StructureType.Mining
	mining.Level = 1
	mining.Size = NormalSampler(4, 2)
	mining.RiverPenalty = Quadratic()
	mining.CoastPenalty = Quadratic()
	mining.SlopePenalty = Quadratic(0, -1, 1)
	mining.ElevationPenalty = Quadratic(0, -1, 1)
	parameters.Cities.Layers:Add(mining)
end

Mine3 = MapEnvironmentTrait()
Mine3.Key = "environment-trait-structure-mine-3"
Mine3.Name = "Densely Mined"
function Mine3:Apply(environment)
	local parameters = environment.Parameters

	local mining = CityGenerator_Parameters()
	mining.CoreDensity = 0.0008
	mining.CandidateDensity = 0.01
	mining.Type = StructureType.Mining
	mining.Level = 1
	mining.Size = NormalSampler(8, 4)
	mining.RiverPenalty = Quadratic()
	mining.CoastPenalty = Quadratic()
	mining.SlopePenalty = Quadratic(0, -1, 1)
	mining.ElevationPenalty = Quadratic(0, -1, 1)
	parameters.Cities.Layers:Add(mining)
end

Infrastructure1 = MapEnvironmentTrait()
Infrastructure1.Key = "environment-trait-structure-infrastructure-1"
Infrastructure1.Name = "Basic Infrastructure"
function Infrastructure1:Apply(environment)
	local parameters = environment.Parameters

	local roads = TransportGenerator_Parameters()
	roads.Type = EdgeType.Road
	roads.Level = 1
	roads.SupportedStructures:Add(
		StructureType.Commercial, TransportGenerator_StructurePredicate(1, false))
	roads.SupportedStructures:Add(
		StructureType.Residential, TransportGenerator_StructurePredicate(1, false))
	roads.SupportedStructures:Add(
		StructureType.Mining, TransportGenerator_StructurePredicate(1, false))
	roads.SupportedStructures:Add(
		StructureType.Industrial, TransportGenerator_StructurePredicate(1, false))
	roads.SupportedStructures:Add(
		StructureType.Agricultural, TransportGenerator_StructurePredicate(1, true))
	parameters.Transport:Add(roads)
end

function Load()
	return { 
		Habitation0, 
		Habitation1, 
		Habitation2, 
		Habitation3,
		Habitation4, 
		Habitation5,

		Farm0,
		Farm1,
		Farm2,
		Farm3,

		Mine0,
		Mine1,
		Mine2,
		Mine3,

		Infrastructure1
	}
end