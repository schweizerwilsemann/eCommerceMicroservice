using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OrderApi.Application.DTOs
{
    public record AppUserDTO(
        int Id,
        [Required]
        string Name,
        [Required]
        string TelephoneNumber,
        [Required, EmailAddress]
        string Email,
        [Required, EmailAddress]
        string Address,
        [Required]
        string Password,
        [Required]
        string Role
    );
}
