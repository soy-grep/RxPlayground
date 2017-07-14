﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;


namespace RxCsPlayground.PaginatedSearchBox
{

    /// <summary>
    /// Asynchrónny zdroj dát podporujúci stránkovanie
    /// </summary>
    public class PaginatedItemsSource : IPaginatedItemsSource
    {

        /// <summary>
        /// Počet položiek, ktoré prídu v jednej "stránke" výsledkov vyhľadávania
        /// </summary>
        public const int ItemsPerPage = 4;


        /// <summary>
        /// Umelé oneskorenie simulujúce zložité načítavanie dát zo zdroja
        /// </summary>
        public const int FakeLoadDelay = 200;


        /// <summary>
        /// Vyhľadanie položiek obsahujúcich vyhľadávaný výraz
        /// </summary>
        public IObservable<PaginatedSearchResult> GetItems(string searchTerm)
        {
            // Vrátime novú observable koleciu vytvorenú pomocou užitočnej factory metódy.
            // Všetko čo potrebujeme je pár ďalších funkcií...
            return Observable.Generate<PaginatedSearchState, PaginatedSearchResult>(
                new PaginatedSearchState(searchTerm),
                x => x.State != StateVariant.Done, 
                Iterate, 
                GetResult,
                Scheduler.Default);
        }

        
        /// <summary>
        /// Funkcia, ktorá vygeneruje filtrovací predicate podľa aktuálne hľadaného výrazu
        /// </summary>
        private Func<string, bool> _filterPredicate(string searchTerm) =>
            x => x.ToLower().Contains(searchTerm.ToLower());


        /// <summary>
        /// Iterácia po dátach a stránkovanie - predstavuje zmenu aktuálneho stavu na nový stav
        /// </summary>
        /// <remarks>
        /// Zodpovedá delegátovi z metódy <see cref="Observable.Generate{TState, TResult}(TState, Func{TState, bool}, Func{TState, TState}, Func{TState, TResult})"/>
        /// </remarks>
        private PaginatedSearchState Iterate(PaginatedSearchState currentState)
        {
            // simulujeme dlho trvajucu operaciu
            Thread.Sleep(FakeLoadDelay);
                        
            var items = string.IsNullOrWhiteSpace(currentState.SearchTerm)
                ? Enumerable.Empty<string>()
                : _items
                    .Where(_filterPredicate(currentState.SearchTerm))
                    .Skip(currentState.PageNr * ItemsPerPage)
                    .Take(ItemsPerPage);

            var newState = (items.Count() > 0)
                ? StateVariant.ReturningItems 
                : StateVariant.Done;

            var newPageNr = currentState.PageNr + 1;

            // vysledny stav bude pochadzat z aktualneho stavu so zmenenymi hodnotami
            return currentState.ChangeState(newPageNr, items, newState);
        }


        /// <summary>
        /// Vygenerovanie výslednej položky do streamu na základe aktuálneho stavu
        /// </summary>
        /// <remarks>
        /// Zodpovedá delegátovi z metódy <see cref="Observable.Generate{TState, TResult}(TState, Func{TState, bool}, Func{TState, TState}, Func{TState, TResult})"/>
        /// </remarks>
        private PaginatedSearchResult GetResult(PaginatedSearchState currentState)
        {
            Debug.Print($"Source.GetResult ThreadId: {Thread.CurrentThread.ManagedThreadId}");

            if (currentState.State == StateVariant.New)
                return new SearchResult_StartingNewStream(currentState.SearchTerm);
            else
                return new SearchResult_ReturningItems(currentState.SearchTerm, currentState.PageNr, currentState.Items);
        }
                        
        
        #region Interná kolekcia položiek

        private List<string> _items = new List<string>
        {
            "Afghanistan",
            "Albania",
            "Algeria",
            "American Samoa",
            "Andorra",
            "Angola",
            "Anguilla",
            "Antarctica",
            "Antigua and Barbuda",
            "Argentina",
            "Armenia",
            "Aruba",
            "Australia",
            "Austria",
            "Azerbaijan",
            "Bahamas",
            "Bahrain",
            "Bangladesh",
            "Barbados",
            "Belarus",
            "Belgium",
            "Belize",
            "Benin",
            "Bermuda",
            "Bhutan",
            "Bolivia",
            "Bosnia and Herzegovina",
            "Botswana",
            "Bouvet Island",
            "Brazil",
            "British Indian Ocean Territory",
            "Brunei Darussalam",
            "Bulgaria",
            "Burkina Faso",
            "Burundi",
            "Cambodia",
            "Cameroon",
            "Canada",
            "Cape Verde",
            "Cayman Islands",
            "Central African Republic",
            "Chad",
            "Chile",
            "China",
            "Christmas Island",
            "Cocos (Keeling) Islands",
            "Colombia",
            "Comoros",
            "Congo",
            "Congo, the Democratic Republic of the",
            "Cook Islands",
            "Costa Rica",
            "Cote D'Ivoire",
            "Croatia",
            "Cuba",
            "Cyprus",
            "Czech Republic",
            "Denmark",
            "Djibouti",
            "Dominica",
            "Dominican Republic",
            "Ecuador",
            "Egypt",
            "El Salvador",
            "Equatorial Guinea",
            "Eritrea",
            "Estonia",
            "Ethiopia",
            "Falkland Islands (Malvinas)",
            "Faroe Islands",
            "Fiji",
            "Finland",
            "France",
            "French Guiana",
            "French Polynesia",
            "French Southern Territories",
            "Gabon",
            "Gambia",
            "Georgia",
            "Germany",
            "Ghana",
            "Gibraltar",
            "Greece",
            "Greenland",
            "Grenada",
            "Guadeloupe",
            "Guam",
            "Guatemala",
            "Guinea",
            "Guinea-Bissau",
            "Guyana",
            "Haiti",
            "Heard Island and Mcdonald Islands",
            "Holy See (Vatican City State)",
            "Honduras",
            "Hong Kong",
            "Hungary",
            "Iceland",
            "India",
            "Indonesia",
            "Iran, Islamic Republic of",
            "Iraq",
            "Ireland",
            "Israel",
            "Italy",
            "Jamaica",
            "Japan",
            "Jordan",
            "Kazakhstan",
            "Kenya",
            "Kiribati",
            "Korea, Democratic People's Republic of",
            "Korea, Republic of",
            "Kuwait",
            "Kyrgyzstan",
            "Lao People's Democratic Republic",
            "Latvia",
            "Lebanon",
            "Lesotho",
            "Liberia",
            "Libyan Arab Jamahiriya",
            "Liechtenstein",
            "Lithuania",
            "Luxembourg",
            "Macao",
            "Macedonia, the Former Yugoslav Republic of",
            "Madagascar",
            "Malawi",
            "Malaysia",
            "Maldives",
            "Mali",
            "Malta",
            "Marshall Islands",
            "Martinique",
            "Mauritania",
            "Mauritius",
            "Mayotte",
            "Mexico",
            "Micronesia, Federated States of",
            "Moldova, Republic of",
            "Monaco",
            "Mongolia",
            "Montserrat",
            "Morocco",
            "Mozambique",
            "Myanmar",
            "Namibia",
            "Nauru",
            "Nepal",
            "Netherlands",
            "Netherlands Antilles",
            "New Caledonia",
            "New Zealand",
            "Nicaragua",
            "Niger",
            "Nigeria",
            "Niue",
            "Norfolk Island",
            "Northern Mariana Islands",
            "Norway",
            "Oman",
            "Pakistan",
            "Palau",
            "Palestinian Territory, Occupied",
            "Panama",
            "Papua New Guinea",
            "Paraguay",
            "Peru",
            "Philippines",
            "Pitcairn",
            "Poland",
            "Portugal",
            "Puerto Rico",
            "Qatar",
            "Reunion",
            "Romania",
            "Russian Federation",
            "Rwanda",
            "Saint Helena",
            "Saint Kitts and Nevis",
            "Saint Lucia",
            "Saint Pierre and Miquelon",
            "Saint Vincent and the Grenadines",
            "Samoa",
            "San Marino",
            "Sao Tome and Principe",
            "Saudi Arabia",
            "Senegal",
            "Serbia and Montenegro",
            "Seychelles",
            "Sierra Leone",
            "Singapore",
            "Slovakia",
            "Slovenia",
            "Solomon Islands",
            "Somalia",
            "South Africa",
            "South Georgia and the South Sandwich Islands",
            "Spain",
            "Sri Lanka",
            "Sudan",
            "Suriname",
            "Svalbard and Jan Mayen",
            "Swaziland",
            "Sweden",
            "Switzerland",
            "Syrian Arab Republic",
            "Taiwan, Province of China",
            "Tajikistan",
            "Tanzania, United Republic of",
            "Thailand",
            "Timor-Leste",
            "Togo",
            "Tokelau",
            "Tonga",
            "Trinidad and Tobago",
            "Tunisia",
            "Turkey",
            "Turkmenistan",
            "Turks and Caicos Islands",
            "Tuvalu",
            "Uganda",
            "Ukraine",
            "United Arab Emirates",
            "United Kingdom",
            "United States",
            "United States Minor Outlying Islands",
            "Uruguay",
            "Uzbekistan",
            "Vanuatu",
            "Venezuela",
            "Viet Nam",
            "Virgin Islands, British",
            "Virgin Islands, US",
            "Wallis and Futuna",
            "Western Sahara",
            "Yemen",
            "Zambia",
            "Zimbabwe"
        };

        #endregion
        
    }

}