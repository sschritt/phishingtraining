using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Enums;

namespace PhishingTraining.Web.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "PhoneNumber")]
            public string PhoneNumber { get; set; }

            [DataType(DataType.Text), Display(Name = "FirstName")]
            public string FirstName { get; set; }

            [DataType(DataType.Text), Display(Name = "LastName")]
            public string LastName { get; set; }

            [Display(Name = "Gender")]
            public GenderType Gender { get; set; }

            [DataType(DataType.Text), Display(Name = "MotherFirstName")]
            public string MotherFirstName { get; set; }

            [DataType(DataType.Text), Display(Name = "FatherFirstName")]
            public string FatherFirstName { get; set; }

            [DataType(DataType.Text), Display(Name = "PetName")]
            public string PetName { get; set; }

            [DataType(DataType.Text), Display(Name = "Street")]
            public string Street { get; set; }

            [DataType(DataType.Text), Display(Name = "City")]
            public string City { get; set; }

            [DataType(DataType.Text), Display(Name = "Country")]
            public string Country { get; set; }

            [DataType(DataType.Date), Display(Name = "Birthdate")]
            public DateTimeOffset? Birthdate { get; set; }

            [DataType(DataType.Text), Display(Name = nameof(InstagramUser))]
            public string InstagramUser { get; set; }
            [DataType(DataType.Text), Display(Name = nameof(FacebookUser))]
            public string FacebookUser { get; set; }
            [DataType(DataType.Text), Display(Name = nameof(TikTokUser))]
            public string TikTokUser { get; set; }

            [Display(Name = nameof(RealData))]
            public bool RealData { get; set; }

            [Display(Name = nameof(PhoneProvider))]
            public PhoneProviderType PhoneProvider { get; set; }

            [Display(Name = "PhoneOS")]
            public PhoneOsType PhoneOs { get; set; }

            [Display(Name = "ComputerOS")]
            public ComputerOsType ComputerOs { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var fatherFirstName = user.FatherFirstName;
            var motherFirstName = user.MotherFirstName;
            var petName = user.PetName;
            var birthdate = user.Birthdate;
            var street = user.Street;
            var city = user.City;
            var country = user.Country;
            var firstName = user.FirstName;
            var lastName = user.LastName;
            var instagramUserName = user.InstagramUser;
            var facebookUserName = user.FacebookUser;
            var tiktokUserName = user.TikTokUser;
            var gender = user.Gender;
            var phoneOs = user.PhoneOs;
            var phoneProvider = user.PhoneProvider;
            var computerOs = user.ComputerOs;
            var realData = user.RealData;

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FatherFirstName = fatherFirstName,
                MotherFirstName = motherFirstName,
                PetName = petName,
                Birthdate = birthdate,
                Street = street,
                Country = country,
                City = city,
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                InstagramUser = instagramUserName,
                FacebookUser = facebookUserName,
                TikTokUser = tiktokUserName,
                PhoneOs = phoneOs,
                PhoneProvider = phoneProvider,
                ComputerOs = computerOs,
                RealData = realData
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (Input.FirstName != user.FirstName) user.FirstName = Input.FirstName;
            if (Input.LastName != user.LastName) user.LastName = Input.LastName;
            if (Input.Gender != user.Gender) user.Gender = Input.Gender;
            if (Input.PetName != user.PetName) user.PetName = Input.PetName;
            if (Input.Birthdate != user.Birthdate) user.Birthdate = Input.Birthdate;
            if (Input.Street != user.Street) user.Street = Input.Street;
            if (Input.Country != user.Country) user.Country = Input.Country;
            if (Input.City != user.City) user.City = Input.City;
            if (Input.FatherFirstName != user.FatherFirstName) user.FatherFirstName = Input.FatherFirstName;
            if (Input.MotherFirstName != user.MotherFirstName) user.MotherFirstName = Input.MotherFirstName;
            if (Input.InstagramUser != user.InstagramUser) user.InstagramUser = Input.InstagramUser;
            if (Input.FacebookUser != user.FacebookUser) user.FacebookUser= Input.FacebookUser;
            if (Input.TikTokUser != user.TikTokUser) user.TikTokUser = Input.TikTokUser;
            if (Input.PhoneOs != user.PhoneOs) user.PhoneOs = Input.PhoneOs;
            if (Input.PhoneProvider != user.PhoneProvider) user.PhoneProvider = Input.PhoneProvider;
            if (Input.ComputerOs != user.ComputerOs) user.ComputerOs = Input.ComputerOs;
            if (Input.RealData != user.RealData) user.RealData = Input.RealData;
            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}