/*!
 * jQuery Validation Plugin v1.17.0
 *
 * https://jqueryvalidation.org/
 *
 * Copyright (c) 2017 Jörn Zaefferer
 * Released under the MIT license
 */
(function( factory ) {
	if ( typeof define === "function" && define.amd ) {
		define( ["jquery", "./jquery.validate"], factory );
	} else if (typeof module === "object" && module.exports) {
		module.exports = factory( require( "jquery" ) );
	} else {
		factory( jQuery );
	}
}(function( $ ) {

( function() {

	function stripHtml( value ) {

		// Remove html tags and space chars
		return value.replace( /<.[^<>]*?>/g, " " ).replace( /&nbsp;|&#160;/gi, " " )

		// Remove punctuation
		.replace( /[.(),;:!?%#$'\"_+=\/\-“”’]*/g, "" );
	}

	$.validator.addMethod( "maxWords", function( value, element, params ) {
		return this.optional( element ) || stripHtml( value ).match( /\b\w+\b/g ).length <= params;
	}, $.validator.format( "Please enter {0} words or less." ) );

	$.validator.addMethod( "minWords", function( value, element, params ) {
		return this.optional( element ) || stripHtml( value ).match( /\b\w+\b/g ).length >= params;
	}, $.validator.format( "Please enter at least {0} words." ) );

	$.validator.addMethod( "rangeWords", function( value, element, params ) {
		var valueStripped = stripHtml( value ),
			regex = /\b\w+\b/g;
		return this.optional( element ) || valueStripped.match( regex ).length >= params[ 0 ] && valueStripped.match( regex ).length <= params[ 1 ];
	}, $.validator.format( "Please enter between {0} and {1} words." ) );

}() );

// Accept a value from a file input based on a required mimetype
$.validator.addMethod( "accept", function( value, element, param ) {

	// Split mime on commas in case we have multiple types we can accept
	var typeParam = typeof param === "string" ? param.replace( /\s/g, "" ) : "image/*",
		optionalValue = this.optional( element ),
		i, file, regex;

	// Element is optional
	if ( optionalValue ) {
		return optionalValue;
	}

	if ( $( element ).attr( "type" ) === "file" ) {

		// Escape string to be used in the regex
		// see: https://stackoverflow.com/questions/3446170/escape-string-for-use-in-javascript-regex
		// Escape also "/*" as "/.*" as a wildcard
		typeParam = typeParam
				.replace( /[\-\[\]\/\{\}\(\)\+\?\.\\\^\$\|]/g, "\\$&" )
				.replace( /,/g, "|" )
				.replace( /\/\*/g, "/.*" );

		// Check if the element has a FileList before checking each file
		if ( element.files && element.files.length ) {
			regex = new RegExp( ".?(" + typeParam + ")$", "i" );
			for ( i = 0; i < element.files.length; i++ ) {
				file = element.files[ i ];

				// Grab the mimetype from the loaded file, verify it matches
				if ( !file.type.match( regex ) ) {
					return false;
				}
			}
		}
	}

	// Either return true because we've validated each file, or because the
	// browser does not support element.files and the FileList feature
	return true;
}, $.validator.format( "Please enter a value with a valid mimetype." ) );

$.validator.addMethod( "alphanumeric", function( value, element ) {
	return this.optional( element ) || /^\w+$/i.test( value );
}, "Letters, numbers, and underscores only please" );

/*
 * Dutch bank account numbers (not 'giro' numbers) have 9 digits
 * and pass the '11 check'.
 * We accept the notation with spaces, as that is common.
 * acceptable: 123456789 or 12 34 56 789
 */
$.validator.addMethod( "bankaccountNL", function( value, element ) {
	if ( this.optional( element ) ) {
		return true;
	}
	if ( !( /^[0-9]{9}|([0-9]{2} ){3}[0-9]{3}$/.test( value ) ) ) {
		return false;
	}

	// Now '11 check'
	var account = value.replace( / /g, "" ), // Remove spaces
		sum = 0,
		len = account.length,
		pos, factor, digit;
	for ( pos = 0; pos < len; pos++ ) {
		factor = len - pos;
		digit = account.substring( pos, pos + 1 );
		sum = sum + factor * digit;
	}
	return sum % 11 === 0;
}, "Please specify a valid bank account number" );

$.validator.addMethod( "bankorgiroaccountNL", function( value, element ) {
	return this.optional( element ) ||
			( $.validator.methods.bankaccountNL.call( this, value, element ) ) ||
			( $.validator.methods.giroaccountNL.call( this, value, element ) );
}, "Please specify a valid bank or giro account number" );

/**
 * BIC is the business identifier code (ISO 9362). This BIC check is not a guarantee for authenticity.
 *
 * BIC pattern: BBBBCCLLbbb (8 or 11 characters long; bbb is optional)
 *
 * Validation is case-insensitive. Please make sure to normalize input yourself.
 *
 * BIC definition in detail:
 * - First 4 characters - bank code (only letters)
 * - Next 2 characters - ISO 3166-1 alpha-2 country code (only letters)
 * - Next 2 characters - location code (letters and digits)
 *   a. shall not start with '0' or '1'
 *   b. second character must be a letter ('O' is not allowed) or digit ('0' for test (therefore not allowed), '1' denoting passive participant, '2' typically reverse-billing)
 * - Last 3 characters - branch code, optional (shall not start with 'X' except in case of 'XXX' for primary office) (letters and digits)
 */
$.validator.addMethod( "bic", function( value, element ) {
    return this.optional( element ) || /^([A-Z]{6}[A-Z2-9][A-NP-Z1-9])(X{3}|[A-WY-Z0-9][A-Z0-9]{2})?$/.test( value.toUpperCase() );
}, "Please specify a valid BIC code" );

/*
 * Código de identificación fiscal ( CIF ) is the tax identification code for Spanish legal entities
 * Further rules can be found in Spanish on http://es.wikipedia.org/wiki/C%C3%B3digo_de_identificaci%C3%B3n_fiscal
 *
 * Spanish CIF structure:
 *
 * [ T ][ P ][ P ][ N ][ N ][ N ][ N ][ N ][ C ]
 *
 * Where:
 *
 * T: 1 character. Kind of Organization Letter: [ABCDEFGHJKLMNPQRSUVW]
 * P: 2 characters. Province.
 * N: 5 characters. Secuencial Number within the province.
 * C: 1 character. Control Digit: [0-9A-J].
 *
 * [ T ]: Kind of Organizations. Possible values:
 *
 *   A. Corporations
 *   B. LLCs
 *   C. General partnerships
 *   D. Companies limited partnerships
 *   E. Communities of goods
 *   F. Cooperative Societies
 *   G. Associations
 *   H. Communities of homeowners in horizontal property regime
 *   J. Civil Societies
 *   K. Old format
 *   L. Old format
 *   M. Old format
 *   N. Nonresident entities
 *   P. Local authorities
 *   Q. Autonomous bodies, state or not, and the like, and congregations and religious institutions
 *   R. Congregations and religious institutions (since 2008 ORDER EHA/451/2008)
 *   S. Organs of State Administration and regions
 *   V. Agrarian Transformation
 *   W. Permanent establishments of non-resident in Spain
 *
 * [ C ]: Control Digit. It can be a number or a letter depending on T value:
 * [ T ]  -->  [ C ]
 * ------    ----------
 *   A         Number
 *   B         Number
 *   E         Number
 *   H         Number
 *   K         Letter
 *   P         Letter