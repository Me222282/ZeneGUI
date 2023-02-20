using System;
using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public abstract class GraphicsManager : IBasicRenderer
    {
        public GraphicsManager(IElement source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            _refProj = new MultiplyMatrix(Matrix.Identity, _refprojRef);
        }

        public IElement Source { get; }

        private bool _inCallback = false;
        private Box _bounds;
        public Box Bounds
        {
            get => _bounds;
            set
            {
                if (!_inCallback)
                {
                    ChangeSize(new VectorEventArgs(value.Size));
                    MoveElement(new VectorEventArgs(value.Location));
                    return;
                }

                _bounds = value;
            }
        }
        public Vector2 Size
        {
            get => _bounds.Size;
            set
            {
                if (!_inCallback)
                {
                    ChangeSize(new VectorEventArgs(value));
                    return;
                }

                _bounds.Size = value;
            }
        }
        public Vector2 Location
        {
            get => _bounds.Location;
            set
            {
                if (!_inCallback)
                {
                    MoveElement(new VectorEventArgs(value));
                    return;
                }

                _bounds.Location = value;
            }
        }

        protected TextRenderer TextRenderer => Source.Properties.handle.TextRenderer;

        public abstract void OnRender(DrawManager context);

        internal void ChangeSize(VectorEventArgs e)
        {
            _inCallback = true;

            OnSizeChange(e);

            UIManager uim = Source.Properties.handle;
            if (uim.renderFocus == Source)
            {
                uim.SetRenderSize(_bounds.Size);
            }
            SetProjection();

            _inCallback = false;
        }
        protected virtual void OnSizeChange(VectorEventArgs e)
        {
            _bounds.Size = e.Value;
        }
        internal void MoveElement(VectorEventArgs e)
        {
            _inCallback = true;

            OnElementMove(e);

            UIManager uim = Source.Properties.handle;
            if (uim.renderFocus == Source)
            {
                uim.SetRenderLocation(_bounds.Location);
            }

            _inCallback = false;
        }
        protected virtual void OnElementMove(VectorEventArgs e)
        {
            _bounds.Location = e.Value;
        }

        internal void SetView()
        {
            _refProj.Left = Matrix4.CreateBox(new Box(Source.Properties.ViewPan, Source.Properties.ViewScale));
        }
        private void SetProjection()
        {
            _refprojRef.Source = Matrix4.CreateOrthographic(_bounds.Width, _bounds.Height, 0d, 1d);
        }
        private readonly MultiplyMatrix _refProj;
        /// <summary>
        /// The projection matrix used to render objects to this element.
        /// </summary>
        public IMatrix Projection => _refProj;
        private readonly ReferenceMatrix _refprojRef = new ReferenceMatrix(Matrix.Identity);
        /// <summary>
        /// The projection matrix used to render rixed objects to this element,
        /// like borders that shouldn't be affected by zoom and panning.
        /// </summary>
        public IMatrix FixedProjection => _refprojRef;
    }
}
