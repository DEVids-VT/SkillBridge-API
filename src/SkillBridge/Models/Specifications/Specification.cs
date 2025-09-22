using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace SkillBridge.Models.Specifications
{
    /// <summary>
    /// Represents the base specification pattern for evaluating conditions on entities of type <typeparamref name="T"/>.
    /// Provides caching of compiled expressions and supports logical combination of specifications.
    /// </summary>
    /// <typeparam name="T">The type of entity to which the specification applies.</typeparam>
    public abstract class Specification<T>
    {
        /// <summary>
        /// Cache for compiled delegates, keyed by a unique cache key per specification.
        /// </summary>
        private static readonly ConcurrentDictionary<string, Func<T, bool>> DelegateCache
            = new ConcurrentDictionary<string, Func<T, bool>>();

        /// <summary>
        /// Tracks the cache key components for uniquely identifying compiled delegates.
        /// </summary>
        private readonly List<string> _cacheKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="Specification{T}"/> class.
        /// </summary>
        protected Specification()
            => this._cacheKey = new List<string> { this.GetType().Name };

        /// <summary>
        /// Indicates whether this specification should be included in evaluation.
        /// Defaults to <c>true</c>.
        /// </summary>
        protected virtual bool Include => true;

        /// <summary>
        /// Evaluates whether the given entity satisfies this specification.
        /// Uses cached compiled delegates for performance optimization.
        /// </summary>
        /// <param name="value">The entity to evaluate.</param>
        /// <returns><c>true</c> if the specification is satisfied; otherwise, <c>false</c>.</returns>
        public virtual bool IsSatisfiedBy(T value)
        {
            if (!this.Include)
            {
                return true;
            }

            var func = DelegateCache.GetOrAdd(
                string.Join(string.Empty, this._cacheKey),
                _ => this.ToExpression().Compile());

            return func(value);
        }

        /// <summary>
        /// Combines this specification with another using a logical AND operator.
        /// </summary>
        /// <param name="specification">The other specification to combine with.</param>
        /// <returns>A new specification representing the logical AND of both.</returns>
        public Specification<T> And(Specification<T> specification)
        {
            if (!specification.Include)
            {
                return this;
            }

            this._cacheKey.Add($"{nameof(this.And)}{specification.GetType()}");

            return new BinarySpecification(this, specification, true);
        }

        /// <summary>
        /// Combines this specification with another using a logical OR operator.
        /// </summary>
        /// <param name="specification">The other specification to combine with.</param>
        /// <returns>A new specification representing the logical OR of both.</returns>
        public Specification<T> Or(Specification<T> specification)
        {
            if (!specification.Include)
            {
                return this;
            }

            this._cacheKey.Add($"{nameof(this.Or)}{specification.GetType()}");

            return new BinarySpecification(this, specification, false);
        }

        /// <summary>
        /// Converts the specification into an expression tree representing its predicate logic.
        /// Must be implemented by concrete specifications.
        /// </summary>
        /// <returns>An expression tree defining the condition.</returns>
        public abstract Expression<Func<T, bool>> ToExpression();

        /// <summary>
        /// Implicitly converts a specification to its underlying expression tree.
        /// If <see cref="Include"/> is false, returns an expression that always evaluates to <c>true</c>.
        /// </summary>
        /// <param name="specification">The specification to convert.</param>
        public static implicit operator Expression<Func<T, bool>>(Specification<T> specification)
            => specification.Include
                ? specification.ToExpression()
                : value => true;

        /// <summary>
        /// Represents a specification that combines two other specifications with a logical operator (AND/OR).
        /// </summary>
        private class BinarySpecification : Specification<T>
        {
            private readonly Specification<T> left;
            private readonly Specification<T> right;
            private readonly bool andOperator;

            /// <summary>
            /// Initializes a new instance of the <see cref="BinarySpecification"/> class.
            /// </summary>
            /// <param name="left">The left specification.</param>
            /// <param name="right">The right specification.</param>
            /// <param name="andOperator">If true, combines with AND; otherwise, with OR.</param>
            public BinarySpecification(Specification<T> left, Specification<T> right, bool andOperator)
            {
                this.right = right;
                this.left = left;
                this.andOperator = andOperator;
            }

            /// <summary>
            /// Builds the expression tree representing the logical combination of the left and right specifications.
            /// </summary>
            /// <returns>The combined expression tree.</returns>
            public override Expression<Func<T, bool>> ToExpression()
            {
                Expression<Func<T, bool>> leftExpression = this.left;
                Expression<Func<T, bool>> rightExpression = this.right;

                Expression body = this.andOperator
                    ? Expression.AndAlso(leftExpression.Body, rightExpression.Body)
                    : Expression.OrElse(leftExpression.Body, rightExpression.Body);

                var parameter = Expression.Parameter(typeof(T));
                body = (BinaryExpression)new ParameterReplacer(parameter).Visit(body);

                body = body ?? throw new InvalidOperationException("Binary expression cannot be null.");

                return Expression.Lambda<Func<T, bool>>(body, parameter);
            }
        }

        /// <summary>
        /// Expression visitor used to replace parameters in expression trees with a single shared parameter.
        /// Ensures compatibility when combining multiple specifications.
        /// </summary>
        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression parameter;

            /// <summary>
            /// Initializes a new instance of the <see cref="ParameterReplacer"/> class.
            /// </summary>
            /// <param name="parameter">The parameter expression to use as a replacement.</param>
            internal ParameterReplacer(ParameterExpression parameter)
                => this.parameter = parameter;

            /// <inheritdoc />
            protected override Expression VisitParameter(ParameterExpression node)
                => base.VisitParameter(this.parameter);
        }
    }
}
