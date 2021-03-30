using CircleMove;

namespace CircleDraw
{
    class CreatePoint : Change
    {
        Figure shape;
        public CreatePoint(int id, Form1 form, Figure shape)
            :base(id, form)
        {
            this.shape = shape;
        }

        public override void Redo()
        {
            form.figures[id] = shape;
            form.Refresh();
        }

        public override void Undo()
        {
            form.figures[id] = null;
            form.Refresh();
        }
    }
}
