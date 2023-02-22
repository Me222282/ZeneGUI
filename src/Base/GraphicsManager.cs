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

        private Box _bounds;
        public Box Bounds
        {
            get => _bounds;
            set
            {
                Size = value.Size;
                Location = value.Location;
            }
        }
        public Vector2 Size
        {
            get => _bounds.Size;
            set
            {
                if (_bounds.Size == value) { return; }

                _bounds.Size = value;

                UIManager uim = Source.Properties.handle;
                if (uim.renderFocus == Source)
                {
                    uim.SetRenderSize(_bounds.Size);
                }
                SetProjection();
                Source.ViewBoxChange();
            }
        }
        public Vector2 Location
        {
            get => _bounds.Location;
            set
            {
                if (_bounds.Location == value) { return; }

                _bounds.Location = value;

                UIManager uim = Source.Properties.handle;
                if (uim.renderFocus == Source)
                {
                    uim.SetRenderLocation(_bounds.Location);
                }
                Source.ViewBoxChange();
            }
        }

        private bool _rwOffset;
        /// <summary>
        /// Determines whether view panning is applied to rendering this object.
        /// </summary>
        public bool RendersWithOffset
        {
            get => _rwOffset;
            set
            {
                if (_rwOffset == value) { return; }

                _rwOffset = value;
                SetView();
            }
        }
        private bool _rwScale;
        /// <summary>
        /// Determines whether view scaling is applied to rendering this object.
        /// </summary>
        public bool RendersWithScale
        {
            get => _rwScale;
            set
            {
                if (_rwScale == value) { return; }

                _rwScale = value;
                SetView();
            }
        }
        protected TextRenderer TextRenderer => Source.Properties.handle.TextRenderer;

        public abstract void OnRender(DrawManager context);

        internal void ChangeSize(VectorEventArgs e) => Size = OnSizeChange(e);
        protected virtual Vector2 OnSizeChange(VectorEventArgs e) => e.Value;
        internal void MoveElement(VectorEventArgs e) => Location = OnElementMove(e);
        protected virtual Vector2 OnElementMove(VectorEventArgs e) => e.Value;

        internal void SetView()
        {
            Vector2 loc = Vector2.Zero;
            if (_rwOffset)
            {
                loc = Source.Properties.ViewPan;
            }
            Vector2 pos = Vector2.One;
            if (_rwScale)
            {
                pos = Source.Properties.ViewScale;
            }

            _refProj.Left = Matrix4.CreateBox(new Box(loc, pos));
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
