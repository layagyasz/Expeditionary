luanet.load_assembly('Cardamom', 'Cardamom.Utils.Generators.Samplers')
luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

CityGenerator_Parameters=luanet.import_type('Expeditionary.Model.Mapping.Generator.CityGenerator+LayerParameters')
EdgeType=luanet.import_type('Expeditionary.Model.Mapping.EdgeType')
MapEnvironmentModifier=luanet.import_type('Expeditionary.Model.Mapping.MapEnvironmentModifier')
NormalSampler=luanet.import_type('Cardamom.Utils.Generators.Samplers.NormalSampler')
Quadratic=luanet.import_type('Expeditionary.Model.Quadratic')
StructureType=luanet.import_type('Expeditionary.Model.Mapping.StructureType')
TransportGenerator_Parameters=luanet.import_type('Expeditionary.Model.Mapping.Generator.TransportGenerator+Parameters')


Habitation0 = MapEnvironmentModifier()
Habitation0.Key = "environment-modifier-structure-habitation-0"
Habitation0.Name = "Uninhabited"
function Habitation0:Apply(environment)
end

Habitation1 = MapEnvironmentModifier()
Habitation1.Key = "environment-modifier-structure-habitation-1"
Habitation1.Name = "Villages"
function Habitation1:Apply(environment)
	local parameters = environment.Parameters

	local commercial = CityGenerator_Parameters()
	commercial.Cores = 10
	commercial.Candidates = 200
	commercial.Type = StructureType.Commercial
	commercial.Size = NormalSampler(2, 1)
	parameters.Cities.Layers:Add(commercial)

	local residential = CityGenerator_Parameters()
	residential.Cores = 40
	residential.Candidates = 200
	residential.Type = StructureType.Residential
	residential.Size = NormalSampler(4, 2)
	parameters.Cities.Layers:Add(residential)

	local industrial = CityGenerator_Parameters()
	industrial.Cores = 10
	industrial.Candidates = 200
	industrial.Type = StructureType.Industrial
	industrial.Size = NormalSampler(1, 1)
	industrial.RiverPenalty = Quadratic()
	parameters.Cities.Layers:Add(industrial)
end

Habitation2 = MapEnvironmentModifier()
Habitation2.Key = "environment-modifier-structure-habitation-2"
Habitation2.Name = "Small Towns"
function Habitation2:Apply(environment)
	local parameters = environment.Parameters

	local commercial = CityGenerator_Parameters()
	commercial.Cores = 10
	commercial.Candidates = 200
	commercial.Type = StructureType.Commercial
	commercial.Size = NormalSampler(4, 2)
	parameters.Cities.Layers:Add(commercial)

	local residential = CityGenerator_Parameters()
	residential.Cores = 40
	residential.Candidates = 200
	residential.Type = StructureType.Residential
	residential.Size = NormalSampler(8, 4)
	parameters.Cities.Layers:Add(residential)

	local industrial = CityGenerator_Parameters()
	industrial.Cores = 10
	industrial.Candidates = 200
	industrial.Type = StructureType.Industrial
	industrial.Size = NormalSampler(2, 1)
	industrial.RiverPenalty = Quadratic()
	parameters.Cities.Layers:Add(industrial)
end

Habitation3 = MapEnvironmentModifier()
Habitation3.Key = "environment-modifier-structure-habitation-3"
Habitation3.Name = "Big Towns"
function Habitation3:Apply(environment)
	local parameters = environment.Parameters

	local commercial = CityGenerator_Parameters()
	commercial.Cores = 10
	commercial.Candidates = 200
	commercial.Type = StructureType.Commercial
	commercial.Size = NormalSampler(16, 8)
	parameters.Cities.Layers:Add(commercial)

	local residential = CityGenerator_Parameters()
	residential.Cores = 40
	residential.Candidates = 200
	residential.Type = StructureType.Residential
	residential.Size = NormalSampler(32, 16)
	parameters.Cities.Layers:Add(residential)

	local industrial = CityGenerator_Parameters()
	industrial.Cores = 10
	industrial.Candidates = 200
	industrial.Type = StructureType.Industrial
	industrial.Size = NormalSampler(8, 4)
	industrial.RiverPenalty = Quadratic()
	parameters.Cities.Layers:Add(industrial)
end

Habitation4 = MapEnvironmentModifier()
Habitation4.Key = "environment-modifier-structure-habitation-4"
Habitation4.Name = "City"
function Habitation4:Apply(environment)
	local parameters = environment.Parameters

	local commercial = CityGenerator_Parameters()
	commercial.Cores = 40
	commercial.Candidates = 200
	commercial.Type = StructureType.Commercial
	commercial.Size = NormalSampler(16, 8)
	parameters.Cities.Layers:Add(commercial)

	local residential = CityGenerator_Parameters()
	residential.Cores = 160
	residential.Candidates = 320
	residential.Type = StructureType.Residential
	residential.Size = NormalSampler(32, 16)
	parameters.Cities.Layers:Add(residential)

	local industrial = CityGenerator_Parameters()
	industrial.Cores = 40
	industrial.Candidates = 200
	industrial.Type = StructureType.Industrial
	industrial.Size = NormalSampler(8, 4)
	industrial.RiverPenalty = Quadratic()
	parameters.Cities.Layers:Add(industrial)
end

Habitation5 = MapEnvironmentModifier()
Habitation5.Key = "environment-modifier-structure-habitation-5"
Habitation5.Name = "Metro"
function Habitation5:Apply(environment)
	local parameters = environment.Parameters

	local commercial = CityGenerator_Parameters()
	commercial.Cores = 80
	commercial.Candidates = 200
	commercial.Type = StructureType.Commercial
	commercial.Size = NormalSampler(16, 8)
	parameters.Cities.Layers:Add(commercial)

	local residential = CityGenerator_Parameters()
	residential.Cores = 320
	residential.Candidates = 640
	residential.Type = StructureType.Residential
	residential.Size = NormalSampler(32, 16)
	parameters.Cities.Layers:Add(residential)

	local industrial = CityGenerator_Parameters()
	industrial.Cores = 80
	industrial.Candidates = 200
	industrial.Type = StructureType.Industrial
	industrial.Size = NormalSampler(8, 4)
	industrial.RiverPenalty = Quadratic()
	parameters.Cities.Layers:Add(industrial)
end

Farm0 = MapEnvironmentModifier()
Farm0.Key = "environment-modifier-structure-farm-0"
Farm0.Name = "Unfarmed"
function Farm0:Apply(environment)
end

Farm1 = MapEnvironmentModifier()
Farm1.Key = "environment-modifier-structure-farm-1"
Farm1.Name = "Sparsely Farmed"
function Farm1:Apply(environment)
	local parameters = environment.Parameters

	local farming = CityGenerator_Parameters()
	farming.Cores = 40
	farming.Candidates = 200
	farming.Type = StructureType.Agricultural
	farming.Size = NormalSampler(20, 10)
	farming.RiverPenalty = Quadratic(0, -2, 2)
	farming.CoastPenalty = Quadratic()
	farming.SiltPenalty = Quadratic(0, -2, 2)
	farming.MoisturePenalty = Quadratic(0, -1, 1)
	parameters.Cities.Layers:Add(farming)
end

Farm2 = MapEnvironmentModifier()
Farm2.Key = "environment-modifier-structure-farm-2"
Farm2.Name = "Moderately Farmed"
function Farm2:Apply(environment)
	local parameters = environment.Parameters

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
end

Farm3 = MapEnvironmentModifier()
Farm3.Key = "environment-modifier-structure-farm-3"
Farm3.Name = "Densely Farmed"
function Farm3:Apply(environment)
	local parameters = environment.Parameters

	local farming = CityGenerator_Parameters()
	farming.Cores = 80
	farming.Candidates = 200
	farming.Type = StructureType.Agricultural
	farming.Size = NormalSampler(40, 20)
	farming.RiverPenalty = Quadratic(0, -2, 2)
	farming.CoastPenalty = Quadratic()
	farming.SiltPenalty = Quadratic(0, -2, 2)
	farming.MoisturePenalty = Quadratic(0, -1, 1)
	parameters.Cities.Layers:Add(farming)
end

Mine0 = MapEnvironmentModifier()
Mine0.Key = "environment-modifier-structure-mine-0"
Mine0.Name = "Unmined"
function Mine0:Apply(environment)
end

Mine1 = MapEnvironmentModifier()
Mine1.Key = "environment-modifier-structure-mine-1"
Mine1.Name = "Sparsely Mined"
function Mine1:Apply(environment)
	local parameters = environment.Parameters

	local mining = CityGenerator_Parameters()
	mining.Cores = 4
	mining.Candidates = 100
	mining.Type = StructureType.Mining
	mining.Level = 1
	mining.Size = NormalSampler(2, 1)
	mining.RiverPenalty = Quadratic()
	mining.CoastPenalty = Quadratic()
	mining.SlopePenalty = Quadratic(0, -1, 1)
	mining.ElevationPenalty = Quadratic(0, -1, 1)
	parameters.Cities.Layers:Add(mining)
end

Mine2 = MapEnvironmentModifier()
Mine2.Key = "environment-modifier-structure-mine-2"
Mine2.Name = "Moderately Mined"
function Mine2:Apply(environment)
	local parameters = environment.Parameters

	local mining = CityGenerator_Parameters()
	mining.Cores = 6
	mining.Candidates = 100
	mining.Type = StructureType.Mining
	mining.Level = 1
	mining.Size = NormalSampler(4, 2)
	mining.RiverPenalty = Quadratic()
	mining.CoastPenalty = Quadratic()
	mining.SlopePenalty = Quadratic(0, -1, 1)
	mining.ElevationPenalty = Quadratic(0, -1, 1)
	parameters.Cities.Layers:Add(mining)
end

Mine3 = MapEnvironmentModifier()
Mine3.Key = "environment-modifier-structure-mine-3"
Mine3.Name = "Densely Mined"
function Mine3:Apply(environment)
	local parameters = environment.Parameters

	local mining = CityGenerator_Parameters()
	mining.Cores = 8
	mining.Candidates = 100
	mining.Type = StructureType.Mining
	mining.Level = 1
	mining.Size = NormalSampler(8, 4)
	mining.RiverPenalty = Quadratic()
	mining.CoastPenalty = Quadratic()
	mining.SlopePenalty = Quadratic(0, -1, 1)
	mining.ElevationPenalty = Quadratic(0, -1, 1)
	parameters.Cities.Layers:Add(mining)
end

Infrastructure1 = MapEnvironmentModifier()
Infrastructure1.Key = "environment-modifier-structure-infrastructure-1"
Infrastructure1.Name = "Basic Infrastructure"
function Infrastructure1:Apply(environment)
	local parameters = environment.Parameters

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