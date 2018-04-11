﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Axoom.MyService.Infrastructure
{
    /// <summary>
    /// Ensures that a block of code running on a background thread cleanly exits before a <see cref="CancellationTokenSource.Cancel()"/> call completes.
    /// </summary>
    /// <remarks>Do not use this if <see cref="CancellationTokenSource.Cancel()"/> is called from the same <see cref="SynchronizationContext"/> the guarded code is running under. This could lead to deadlocks.</remarks>
    /// <example>
    /// This class is best used in a using-block:
    /// <code>
    /// using (new CancellationGuard(cancellationToken))
    /// {
    ///     // Your code
    /// }
    /// </code>
    /// </example>
    public class CancellationGuard : IDisposable
    {
        private CancellationTokenRegistration _registration;

        private readonly TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

        /// <summary>
        /// Registers a callback for the <paramref name="cancellationToken"/> that blocks calls to <see cref="CancellationTokenSource.Cancel()"/> until <see cref="Dispose"/> has been called.
        /// </summary>
        /// <param name="cancellationToken">Used to signal cancellation requests.</param>
        public CancellationGuard(CancellationToken cancellationToken)
            => _registration = cancellationToken.Register(_tcs.Task.Wait);

        /// <summary>
        /// Registers a callback for the <paramref name="cancellationToken"/> that blocks calls to <see cref="CancellationTokenSource.Cancel()"/> until <see cref="Dispose"/> has been called.
        /// </summary>
        /// <param name="cancellationToken">Used to signal cancellation requests.</param>
        /// <param name="timeout">A timespan after which the cancellation will be considered completed even if <see cref="Dispose"/> has not been called yet.</param>
        public CancellationGuard(CancellationToken cancellationToken, TimeSpan timeout)
            => _registration = cancellationToken.Register(() => _tcs.Task.Wait(timeout));

        /// <summary>
        /// Releases the block and allows <see cref="CancellationTokenSource.Cancel()"/> to complete.
        /// </summary>
        public void Dispose()
        {
            _tcs.SetResult(true);
            _registration.Dispose();
        }
    }
}
