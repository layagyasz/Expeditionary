luanet.load_assembly('Cardamom', 'Cardamom.ImageProcessing.Filters')
luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

Evaluator=luanet.import_type('Cardamom.ImageProcessing.Filters.LatticeNoise+Evaluator')
Interpolator=luanet.import_type('Cardamom.ImageProcessing.Filters.LatticeNoise+Interpolator')
MapEnvironmentModifier=luanet.import_type('Expeditionary.Model.Mapping.MapEnvironmentModifier')
Quadratic=luanet.import_type('Expeditionary.Model.Quadratic')
Treatment=luanet.import_type('Cardamom.ImageProcessing.Filters.LatticeNoise+Treatment')

CrystallineA = MapEnvironmentModifier()
CrystallineA.Key = "environment-modifier-landform-crystalline-a"
CrystallineA.Name = "Crystalline A"
function CrystallineA:Apply(environment)
	local parameters = environment.Parameters
	local terrain = parameters.Terrain
	local noise = terrain.ElevationLayer.Noise

	noise.Evaluator = Evaluator.Divergence
	noise.Interpolator = Interpolator.HermiteSigmoid
	noise.PreTreatment = Treatment.None
	noise.PostTreatment = Treatment.Ridge

	terrain.ElevationLayer.Mean = -0.1
	terrain.ElevationLayer.StandardDeviation = 0.08
	terrain.ElevationLayer.Transform = Quadratic(0, 0.5, 0.5)
end

CrystallineB = MapEnvironmentModifier()
CrystallineB.Key = "environment-modifier-landform-crystalline-b"
CrystallineB.Name = "Crystalline B"
function CrystallineB:Apply(environment)
	local parameters = environment.Parameters
	local terrain = parameters.Terrain
	local noise = terrain.ElevationLayer.Noise

	noise.Evaluator = Evaluator.Curl
	noise.Interpolator = Interpolator.HermiteQuintic
	noise.PreTreatment = Treatment.None
	noise.PostTreatment = Treatment.SemiRidge

	terrain.ElevationLayer.Mean = -0.1
	terrain.ElevationLayer.StandardDeviation = 0.07
	terrain.ElevationLayer.Transform = Quadratic(0, 0.5, 0.5)
end

CrystallineC = MapEnvironmentModifier()
CrystallineC.Key = "environment-modifier-landform-crystalline-c"
CrystallineC.Name = "Crystalline C"
function CrystallineC:Apply(environment)
	local parameters = environment.Parameters
	local terrain = parameters.Terrain
	local noise = terrain.ElevationLayer.Noise

	noise.Evaluator = Evaluator.ParabolicInverse
	noise.Interpolator = Interpolator.Linear
	noise.PreTreatment = Treatment.None
	noise.PostTreatment = Treatment.None

	terrain.ElevationLayer.Mean = 0.3
	terrain.ElevationLayer.StandardDeviation = 0.3
	terrain.ElevationLayer.Transform = Quadratic(0, 0.5, 0.5)
end

CrystallineD = MapEnvironmentModifier()
CrystallineD.Key = "environment-modifier-landform-crystalline-d"
CrystallineD.Name = "Crystalline D"
function CrystallineD:Apply(environment)
	local parameters = environment.Parameters
	local terrain = parameters.Terrain
	local noise = terrain.ElevationLayer.Noise

	noise.Evaluator = Evaluator.VerticalEdgeInverse
	noise.Interpolator = Interpolator.Linear
	noise.PreTreatment = Treatment.SemiRidge
	noise.PostTreatment = Treatment.None

	terrain.ElevationLayer.Mean = -0.15
	terrain.ElevationLayer.StandardDeviation = 0.12
	terrain.ElevationLayer.Transform = Quadratic(0, 0.5, 0.5)
end

LithicA = MapEnvironmentModifier()
LithicA.Key = "environment-modifier-landform-lithic-a"
LithicA.Name = "Lithic A"
function LithicA:Apply(environment)
	local parameters = environment.Parameters
	local terrain = parameters.Terrain
	local noise = terrain.ElevationLayer.Noise

	noise.Evaluator = Evaluator.Gradient
	noise.Interpolator = Interpolator.Linear
	noise.PreTreatment = Treatment.None
	noise.PostTreatment = Treatment.None

	terrain.ElevationLayer.Mean = 0
	terrain.ElevationLayer.StandardDeviation = 0.2
	terrain.ElevationLayer.Transform = Quadratic(0, 0.5, 0.5)
end

LithicB = MapEnvironmentModifier()
LithicB.Key = "environment-modifier-landform-lithic-b"
LithicB.Name = "Lithic B"
function LithicB:Apply(environment)
	local parameters = environment.Parameters
	local terrain = parameters.Terrain
	local noise = terrain.ElevationLayer.Noise

	noise.Evaluator = Evaluator.TriangularEdge
	noise.Interpolator = Interpolator.Cosine
	noise.PreTreatment = Treatment.None
	noise.PostTreatment = Treatment.Billow

	terrain.ElevationLayer.Mean = 0.16
	terrain.ElevationLayer.StandardDeviation = 0.06
	terrain.ElevationLayer.Transform = Quadratic(0, 0.5, 0.5)
end

LithicC = MapEnvironmentModifier()
LithicC.Key = "environment-modifier-landform-lithic-c"
LithicC.Name = "Lithic C"
function LithicC:Apply(environment)
	local parameters = environment.Parameters
	local terrain = parameters.Terrain
	local noise = terrain.ElevationLayer.Noise

	noise.Evaluator = Evaluator.VerticalEdgeInverseDisplaced
	noise.Interpolator = Interpolator.Hermite
	noise.PreTreatment = Treatment.SemiRidge
	noise.PostTreatment = Treatment.Billow

	terrain.ElevationLayer.Mean = 0.2
	terrain.ElevationLayer.StandardDeviation = 0.1
	terrain.ElevationLayer.Transform = Quadratic(0, 0.5, 0.5)
end

LithicD = MapEnvironmentModifier()
LithicD.Key = "environment-modifier-landform-lithic-d"
LithicD.Name = "Lithic D"
function LithicD:Apply(environment)
	local parameters = environment.Parameters
	local terrain = parameters.Terrain
	local noise = terrain.ElevationLayer.Noise

	noise.Evaluator = Evaluator.VerticalEdgeInverseDisplaced
	noise.Interpolator = Interpolator.Triweight
	noise.PreTreatment = Treatment.None
	noise.PostTreatment = Treatment.None

	terrain.ElevationLayer.Mean = 0.5
	terrain.ElevationLayer.StandardDeviation = 0.5
	terrain.ElevationLayer.Transform = Quadratic(0, 0.5, 0.5)
end

function Load()
	return { CrystallineA, CrystallineB, CrystallineC, CrystallineD, LithicA, LithicB, LithicC, LithicD }
end