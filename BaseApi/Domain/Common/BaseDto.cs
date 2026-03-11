using System;

namespace BaseApi.Domain.Common;

public abstract class BaseDto<TId>
{
    public TId Id { get; set; } = default!;
}

public abstract class BaseDto : BaseDto<Guid>
{
}
