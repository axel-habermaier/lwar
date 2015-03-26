namespace Graphics
{
	template <typename T>
	bool ChangeState(T* stateValue, const T value)
	{
		if (*stateValue == value)
			return false;

		*stateValue = value;
		return true;
	}
}