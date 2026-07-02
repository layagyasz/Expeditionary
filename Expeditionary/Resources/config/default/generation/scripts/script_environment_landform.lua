luanet.load_assembly('Cardamom', 'Cardamom.ImageProcessing.Filters')
luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

Evaluator=luanet.import_type('Cardamom.ImageProcessing.Filters.LatticeNoise+Evaluator')
Interpolator=luanet.import_type('Cardamom.ImageProcessing.Filters.LatticeNoise+Interpolator')
MapEnvironmentTrait=luanet.import_type('Expeditionary.Model.Mapping.Environments.MapEnvironmentTrait')
Quadratic=luanet.import_type('Expeditionary.Model.Quadratic')
Treatment=luanet.import_type('Cardamom.ImageProcessing.Filters.LatticeNoise+Treatment')

CrystallineA = MapEnvironmentTrait()
CrystallineA.Key = "environment-trait-landform-crystalline-a"
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

CrystallineB = MapEnvironmentTrait()
CrystallineB.Key = "environment-trait-landform-crystalline-b"
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

CrystallineC = MapEnvironmentTrait()
CrystallineC.Key = "environment-trait-landform-crystalline-c"
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

CrystallineD = MapEnvironmentTrait()
CrystallineD.Key = "environment-trait-landform-crystalline-d"
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

LithicA = MapEnvironmentTrait()
LithicA.Key = "environment-trait-landform-lithic-a"
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

LithicB = MapEnvironmentTrait()
LithicB.Key = "environment-trait-landform-lithic-b"
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

LithicC = MapEnvironmentTrait()
LithicC.Key = "environment-trait-landform-lithic-c"
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

LithicD = MapEnvironmentTrait()
LithicD.Key = "environment-trait-landform-lithic-d"
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