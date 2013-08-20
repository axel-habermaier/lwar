using System;

namespace Pegasus.Framework.Platform.Performance
{
	using System.Text;
	using Graphics;
	using Memory;

	/// <summary>
	///   Manages timestamp queries to profile the time the GPU spends on rendering each frame.
	/// </summary>
	internal class GraphicsDeviceProfiler : DisposableObject, IMeasurement
	{
		/// <summary>
		///   The number of samples for the computation of the average.
		/// </summary>
		private const int AverageSamples = 32;

		/// <summary>
		///   The number of queries that is used to compute the frame time. As the CPU and the GPU work asynchronously,
		///   the results of the queries are not immediately available. In order to avoid stalling the CPU, the results
		///   of the queries are checked BufferSize frames later.
		/// </summary>
		private const int BufferSize = 3;

		/// <summary>
		///   The timestamp queries that mark the beginning of a frame.
		/// </summary>
		private readonly TimestampQuery[] _beginQueries = new TimestampQuery[BufferSize];

		/// <summary>
		///   The number of blend state binding changes that have been made.
		/// </summary>
		private readonly AveragedInteger _blendStateBindingCount = new AveragedInteger(AverageSamples);

		/// <summary>
		///   The number of buffer mapping operations that have been made.
		/// </summary>
		private readonly AveragedInteger _bufferMapCount = new AveragedInteger(AverageSamples);

		/// <summary>
		///   The number of constant buffer binding changes that have been made.
		/// </summary>
		private readonly AveragedInteger _constantBufferBindingCount = new AveragedInteger(AverageSamples);

		/// <summary>
		///   The number of constant buffer updates that have been made.
		/// </summary>
		private readonly AveragedInteger _constantBufferUpdates = new AveragedInteger(AverageSamples);

		/// <summary>
		///   The number of depth stencil state binding changes that have been made.
		/// </summary>
		private readonly AveragedInteger _depthStencilStateBindingCount = new AveragedInteger(AverageSamples);

		/// <summary>
		///   The timestamp disjoint queries that are used to check whether the timestamps are valid and that allow the
		///   correct interpretation of the timestamp values.
		/// </summary>
		private readonly TimestampDisjointQuery[] _disjointQueries = new TimestampDisjointQuery[BufferSize];

		/// <summary>
		///   The number of draw calls that have been made.
		/// </summary>
		private readonly AveragedInteger _drawCalls = new AveragedInteger(AverageSamples);

		/// <summary>
		///   The timestamp queries that mark the end of a frame.
		/// </summary>
		private readonly TimestampQuery[] _endQueries = new TimestampQuery[BufferSize];

		/// <summary>
		///   The graphics device that is profiled.
		/// </summary>
		private readonly GraphicsDevice _graphicsDevice;

		/// <summary>
		///   The number of input layout binding changes that have been made.
		/// </summary>
		private readonly AveragedInteger _inputLayoutBindingCount = new AveragedInteger(AverageSamples);

		/// <summary>
		///   The number of rasterizer state binding changes that have been made.
		/// </summary>
		private readonly AveragedInteger _rasterizerStateBindingCount = new AveragedInteger(AverageSamples);

		/// <summary>
		///   The number of render target binding changes that have been made.
		/// </summary>
		private readonly AveragedInteger _renderTargetBindingCount = new AveragedInteger(AverageSamples);

		/// <summary>
		///   Gets the statistical render time information.
		/// </summary>
		private readonly AveragedDouble _renderTime = new AveragedDouble("ms", AverageSamples);

		/// <summary>
		///   The number of sampler staste binding changes that have been made.
		/// </summary>
		private readonly AveragedInteger _samplerStateBindingCount = new AveragedInteger(AverageSamples);

		/// <summary>
		///   The number of shader binding changes that have been made.
		/// </summary>
		private readonly AveragedInteger _shaderBindingCount = new AveragedInteger(AverageSamples);

		/// <summary>
		///   The number of texture binding changes that have been made.
		/// </summary>
		private readonly AveragedInteger _textureBindingCount = new AveragedInteger(AverageSamples);

		/// <summary>
		///   The number of vertices that have been drawn.
		/// </summary>
		private readonly AveragedInteger _vertexCount = new AveragedInteger(AverageSamples);

		/// <summary>
		///   The index denoting the queries whose results that are checked next.
		/// </summary>
		private int _index;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be profiled.</param>
		public GraphicsDeviceProfiler(GraphicsDevice graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			_graphicsDevice = graphicsDevice;

			for (var i = 0; i < BufferSize; ++i)
			{
				_beginQueries[i] = new TimestampQuery(graphicsDevice);
				_beginQueries[i].SetName("GraphicsDeviceProfiler.BeginQuery" + i);
			}

			for (var i = 0; i < BufferSize; ++i)
			{
				_endQueries[i] = new TimestampQuery(graphicsDevice);
				_endQueries[i].SetName("GraphicsDeviceProfiler.EndQuery" + i);
			}

			for (var i = 0; i < BufferSize; ++i)
			{
				_disjointQueries[i] = new TimestampDisjointQuery(graphicsDevice);
				_disjointQueries[i].SetName("GraphicsDeviceProfiler.DisjointQuery" + i);
			}
		}

		/// <summary>
		///   Gets the index of the queries that should be issued during the current frame.
		/// </summary>
		private int StartIndex
		{
			get { return (_index + (BufferSize - 1)) % BufferSize; }
		}

		/// <summary>
		///   Gets the index of the queries whose results should be checked during the current frame.
		/// </summary>
		private int ResultIndex
		{
			get { return _index % BufferSize; }
		}

		/// <summary>
		///   Marks the beginning of a new frame.
		/// </summary>
		public void Begin()
		{
			_disjointQueries[StartIndex].Begin();
			_beginQueries[StartIndex].Query();
		}

		/// <summary>
		///   Marks the end of the current frame.
		/// </summary>
		public void End()
		{
			_endQueries[StartIndex].Query();
			_disjointQueries[StartIndex].End();

			// Skip the first couple of frames for which we do not have any queued queries
			if (_index >= BufferSize - 1)
			{
				// If the queries are not yet available, we have to stall the CPU
				_disjointQueries[ResultIndex].WaitForCompletion();
				_endQueries[ResultIndex].WaitForCompletion();

				// The timestamps might be invalid if the GPU changed its clockrate, for instance
				var result = _disjointQueries[ResultIndex].Result;
				if (!result.Valid)
					return;

				var value = (_endQueries[ResultIndex].Timestamp - _beginQueries[ResultIndex].Timestamp) / (double)result.Frequency * 1000;
				_renderTime.AddMeasurement(value);
			}

			++_index;

			var statistics = _graphicsDevice.GetStatistics();

			_drawCalls.AddMeasurement(statistics.DrawCalls);
			_vertexCount.AddMeasurement(statistics.VertexCount);
			_renderTargetBindingCount.AddMeasurement(statistics.RenderTargetBindingCount);
			_textureBindingCount.AddMeasurement(statistics.TextureBindingCount);
			_constantBufferBindingCount.AddMeasurement(statistics.ConstantBufferBindingCount);
			_constantBufferUpdates.AddMeasurement(statistics.ConstantBufferUpdates);
			_bufferMapCount.AddMeasurement(statistics.BufferMapCount);
			_shaderBindingCount.AddMeasurement(statistics.ShaderBindingCount);
			_inputLayoutBindingCount.AddMeasurement(statistics.InputLayoutBindingCount);
			_blendStateBindingCount.AddMeasurement(statistics.BlendStateBindingCount);
			_depthStencilStateBindingCount.AddMeasurement(statistics.DepthStencilStateBindingCount);
			_samplerStateBindingCount.AddMeasurement(statistics.SamplerStateBindingCount);
			_rasterizerStateBindingCount.AddMeasurement(statistics.RasterizerStateBindingCount);
		}

		/// <summary>
		///   Writes the results of the frame time measurement into the given string builder.
		/// </summary>
		/// <param name="builder">The string builder the results should be written to.</param>
		public void WriteResults(StringBuilder builder)
		{
			_renderTime.WriteResults(builder);
		}

		/// <summary>
		///   Writes the graphics device state changes into the given string builder.
		/// </summary>
		/// <param name="builder">The string builder the results should be written to.</param>
		public void WriteFrameInfo(StringBuilder builder)
		{
			WriteValue(builder, _drawCalls, "   Draw Calls:               ");
			WriteValue(builder, _vertexCount, "   Vertex Count:             ");
			WriteValue(builder, _constantBufferUpdates, "   Constant Buffer Updates:  ");
			WriteValue(builder, _bufferMapCount, "   Other Buffer Updates:     ");
		}

		/// <summary>
		///   Writes the graphics device state changes into the given string builder.
		/// </summary>
		/// <param name="builder">The string builder the results should be written to.</param>
		public void WriteStateChanges(StringBuilder builder)
		{
			builder.Append("State Changes");
			WriteValue(builder, _renderTargetBindingCount, "   Render Targets:           ");
			WriteValue(builder, _textureBindingCount, "   Textures:                 ");
			WriteValue(builder, _shaderBindingCount, "   Shaders:                  ");
			WriteValue(builder, _constantBufferBindingCount, "   Constant Buffers:         ");
			WriteValue(builder, _inputLayoutBindingCount, "   Input Layouts:            ");

			builder.Append('\n');
			WriteValue(builder, _blendStateBindingCount, "   Blend States:             ");
			WriteValue(builder, _depthStencilStateBindingCount, "   Depth Stencil States:     ");
			WriteValue(builder, _samplerStateBindingCount, "   Sampler States:           ");
			WriteValue(builder, _rasterizerStateBindingCount, "   Rasterizer States:        ");
		}

		/// <summary>
		///   Writes the labeled value of the given averaged integer to the given string builder.
		/// </summary>
		/// <param name="builder">The builder that the value should be written to.</param>
		/// <param name="value">The averaged integer that should be written to the string builder.</param>
		/// <param name="label">The label that describes the value.</param>
		private static void WriteValue(StringBuilder builder, AveragedInteger value, string label)
		{
			Assert.ArgumentNotNull(builder);
			Assert.ArgumentNotNull(value);
			Assert.ArgumentNotNullOrWhitespace(label);

			builder.Append('\n');
			builder.Append(label);
			value.WriteResults(builder);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_beginQueries.SafeDisposeAll();
			_endQueries.SafeDisposeAll();
			_disjointQueries.SafeDisposeAll();
		}
	}
}