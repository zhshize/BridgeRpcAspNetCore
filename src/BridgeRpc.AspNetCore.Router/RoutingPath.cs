using System;
using System.Linq;
using BridgeRpc.Core;

namespace BridgeRpc.AspNetCore.Router
{
	/// <summary>
	/// Represents the url path for Rpc routing purposes
	/// </summary>
	public struct RoutingPath : IEquatable<RoutingPath>
	{
		/// <summary>
		/// Default/Empty path
		/// </summary>
		public static RoutingPath Default => new RoutingPath();

		/// <summary>
		/// Path components split on forward slashes
		/// </summary>
		private readonly string[] _componentsValue;

		private int? _hashCodeCache;

		/// <param name="components">Uri components for the path</param>
		private RoutingPath(string[] components = null)
		{
			_componentsValue = components ?? new string[0];
			_hashCodeCache = null;
		}

		public static bool operator ==(RoutingPath path1, RoutingPath path2)
		{
			return path1.Equals(path2);
		}

		public static bool operator !=(RoutingPath path1, RoutingPath path2)
		{
			return !path1.Equals(path2);
		}

		public bool StartsWith(RoutingPath other)
		{
			if ((other._componentsValue?.Length ?? 0) == 0)
			{
				return true;
			}

			if ((_componentsValue?.Length ?? 0) == 0)
			{
				return false;
			}

			if (other._componentsValue.Length > _componentsValue.Length)
			{
				return false;
			}

			for (int i = 0; i < other._componentsValue.Length; i++)
			{
				string component = _componentsValue[i];
				string otherComponent = other._componentsValue[i];
				if (!string.Equals(component, otherComponent))
				{
					return false;
				}
			}

			return true;
		}

		public bool Equals(RoutingPath other)
		{
			return GetHashCode() == other.GetHashCode();
		}


		public override bool Equals(object obj)
		{
			if (obj is RoutingPath path)
			{
				return Equals(path);
			}

			return false;
		}


		public override int GetHashCode()
		{
			if (_hashCodeCache != null) return _hashCodeCache.Value;
			int hash;
			if (_componentsValue == null || _componentsValue.Length == 0)
			{
				hash = 0;
			}
			else
			{
				hash = _componentsValue.Aggregate(1337, (current, component) => (current * 7) + component.GetHashCode());
			}

			_hashCodeCache = hash;
			return _hashCodeCache.Value;
		}

		/// <summary>
		/// Creates a <see cref="RoutingPath"/> based on the string form of the path
		/// </summary>
		/// <param name="path">Uri/route path</param>
		/// <returns>Rpc path based on the path string</returns>
		public static RoutingPath Parse(string path)
		{
			if (!TryParse(path, out var outPath))
			{
				throw new RpcException(RpcErrorCode.ParseError, $"Rpc path could not be parsed from '{path}'.");
			}

			return outPath;
		}

		/// <summary>
		/// Creates a <see cref="RoutingPath"/> based on the string form of the path
		/// </summary>
		/// <param name="path">Uri/route path</param>
		/// <param name="routingPath">Parsed <see cref="RoutingPath"/> object</param>
		/// <returns>True if the path parses, otherwise false</returns>
		public static bool TryParse(string path, out RoutingPath routingPath)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				routingPath = new RoutingPath();
				return true;
			}
			else
			{
				try
				{
					string[] pathComponents = path
						.ToLowerInvariant()
						.Split(new[] {'/', '\\'}, StringSplitOptions.RemoveEmptyEntries);
					routingPath = new RoutingPath(pathComponents);
					return true;
				}
				catch
				{
					routingPath = default;
					return false;
				}
			}
		}

		/// <summary>
		/// Removes the base path path from this path
		/// </summary>
		/// <param name="basePath">Base path to remove</param>
		/// <returns>A new path that is the full path without the base path</returns>
		public RoutingPath RemoveBasePath(RoutingPath basePath)
		{
			if (!TryRemoveBasePath(basePath, out RoutingPath path))
			{
				throw new RpcException(RpcErrorCode.ParseError,
					$"Count not remove path '{basePath}' from path '{this}'.");
			}

			return path;
		}

		/// <summary>
		/// Tries to remove the base path path from this path
		/// </summary>
		/// <param name="basePath">Base path to remove</param>
		/// <param name="path">Removed base uri <see cref="RoutingPath"/> object</param>
		/// <returns>True if removed the base path. Otherwise false</returns>
		public bool TryRemoveBasePath(RoutingPath basePath, out RoutingPath path)
		{
			if (basePath == default)
			{
				path = Clone();
				return true;
			}

			if (!StartsWith(basePath))
			{
				path = default;
				return false;
			}

			var newComponents = new string[_componentsValue.Length - basePath._componentsValue.Length];
			if (newComponents.Length > 0)
			{
				Array.Copy(_componentsValue, basePath._componentsValue.Length, newComponents, 0, newComponents.Length);
			}

			path = new RoutingPath(newComponents);
			return true;
		}

		/// <summary>
		/// Merges the two paths to create a new Rpc path that is the combination of the two
		/// </summary>
		/// <param name="other">Other path to add to the end of the current path</param>
		/// <returns>A new path that is the combination of the two paths</returns>
		public RoutingPath Add(RoutingPath other)
		{
			if (other._componentsValue == null)
			{
				return Clone();
			}

			if (_componentsValue == null)
			{
				return other.Clone();
			}

			int componentCount = _componentsValue.Length + other._componentsValue.Length;
			string[] newComponents = new string[componentCount];
			_componentsValue.CopyTo(newComponents, 0);
			other._componentsValue.CopyTo(newComponents, _componentsValue.Length);
			return new RoutingPath(newComponents);
		}

		public override string ToString()
		{
			if (_componentsValue == null)
			{
				return "/";
			}

			return "/" + string.Join("/", _componentsValue);
		}

		public RoutingPath Clone()
		{
			if (_componentsValue == null || _componentsValue.Length == 0)
			{
				return new RoutingPath();
			}

			var componentCount = _componentsValue.Length;
			var newComponents = new string[componentCount];
			_componentsValue.CopyTo(newComponents, 0);
			return new RoutingPath(newComponents);
		}

		public static implicit operator string(RoutingPath path)
		{
			return path.ToString();
		}

		public static implicit operator RoutingPath(string s)
		{
			return Parse(s);
		}
	}
}