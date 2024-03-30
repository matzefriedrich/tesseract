namespace Tesseract.Tests
{
    using Abstractions;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class DisposableBaseTest
    {
        [Test]
        public void DisposableBase_Dispose_correctly_propagates_disposing_state_Test()
        {
            // Arrange
            var eventSinkMock = new Mock<IDisposeEventSink>();
            eventSinkMock.Setup(sink => sink.Dispose()).Verifiable();

            var sut = new DisposableStub(eventSinkMock.Object);

            // Act
            sut.Dispose();
            sut.Dispose();

            // Assert
            eventSinkMock.Verify(sink => sink.Dispose(), Times.Once);
        }

        [Test]
        public void DisposableBase_Disposed_unsubscribe_event_listeners_on_dispose_Test()
        {
            // Arrange
            var eventSinkMock = new Mock<IDisposeEventSink>();
            eventSinkMock.Setup(sink => sink.Dispose()).Verifiable();

            const int expectedTotalEvents = 1;

            DisposeHandler eventHandler;
            using (var sut = new DisposableStub(eventSinkMock.Object))
            {
                eventHandler = new DisposeHandler(sut);

                // Act
                sut.Dispose();
            }
            
            // Assert
            eventSinkMock.Verify(sink => sink.Dispose(), Times.Once);
            Assert.AreEqual(expectedTotalEvents, eventHandler.TotalEvents, "The Dispose event was not raised on the target object.");
        }

        private sealed class DisposeHandler
        {
            private int counter;
            
            public DisposeHandler(DisposableBase target)
            {
                target.Disposed += this.HandleDispose;
            }

            public int TotalEvents => this.counter;

            private void HandleDispose(object? sender, EventArgs args)
            {
                Interlocked.Increment(ref this.counter);
            }
        }

        private sealed class DisposableStub(IDisposeEventSink eventSink) : DisposableBase
        {
            protected override void Dispose(bool disposing)
            {
                if (this.IsDisposed == false && disposing) eventSink.Dispose();
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public interface IDisposeEventSink
        {
            void Dispose();
        }
    }
}