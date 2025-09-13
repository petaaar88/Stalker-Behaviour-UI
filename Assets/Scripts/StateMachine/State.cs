
using Unity.VisualScripting;

public interface State<T> 
{
    public void Enter(T entity);
    public void Update(T entity);
    public void Exit(T entity);
}
